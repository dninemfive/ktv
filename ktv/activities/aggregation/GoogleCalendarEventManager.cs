using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;

namespace d9.ktv;
public class GoogleCalendarEventManager
{
    public ActivityAggregationConfig Config { get; private set; }
    private readonly Dictionary<Activity, DateTime> _startTimes = new();
    private readonly Dictionary<Activity, string?> _eventIds = new();
    private GoogleCalendarEventManager(ActivityAggregationConfig cfg)
        => Config = cfg;
    public static GoogleCalendarEventManager? From(ActivityAggregationConfig config)
    {
        if (config.GoogleCalendar is null)
            return null;
        return new(config);
    }
    public void PostFromSummary(ActivitySummary summary, float threshold = 0.3f)
    {
        foreach (Activity activity in summary)
        {
            if (!_startTimes.TryGetValue(activity, out DateTime start))
            {
                start = summary.Start;
                _startTimes[activity] = start;
            }
            if (summary[activity] < threshold)
            {
                Remove(activity);
                continue;
            }
            _eventIds[activity] = TryPostEvent(activity, start, summary.End);
        }
    }
    private void Remove(Activity activity)
    {
        _startTimes.Remove(activity);
        _eventIds.Remove(activity);
    }
    public string? TryPostEvent(Activity activity, DateTime start, DateTime end)
    {
        _eventIds.TryGetValue(activity, out string? existingId);
        Event @event = activity.ToEvent(start, end, Config.GoogleCalendar!.ColorFor(activity.Category));
        string calendarId = Config.GoogleCalendar!.Id;
        try
        {
            return existingId is null ? GoogleUtils.AddEventTo(calendarId, @event).Id
                                      : GoogleUtils.UpdateEvent(calendarId, existingId, @event).Id;
        } 
        catch(Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().Name}: {e.Message}");
        }
        return null;
    }
}

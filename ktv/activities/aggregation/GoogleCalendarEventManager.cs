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
    public void PostFromSummary(ActivitySummary summary)
    {
        foreach (Activity activity in summary)
        {
            if (!_startTimes.TryGetValue(activity, out DateTime start))
            {
                start = summary.Start;
                _startTimes[activity] = start;
            }
            if (summary[activity] < Config.GoogleCalendar?.ActivityPercentageThreshold)
            {
                Remove(activity);
                continue;
            }
            _eventIds[activity] = TryPostEvent(activity, start, summary.End);
        }
    }
    private void Remove(Activity activity)
    {
        void printDicts(string msg)
        {
            Console.WriteLine(msg);
            foreach (Activity key in _startTimes.Keys.Union(_eventIds.Keys)
                                                     .Distinct()
                                                     .OrderBy(x => x.Category)
                                                     .ThenBy(x => x.Name))
            {
                Console.WriteLine($"{key,-20}\t{(_startTimes.TryGetValue(key, out DateTime value) ? value : ""),-20}" +
                                  $"\t{(_eventIds.TryGetValue(key, out string? id) ? id : "")}");
            }
        }
        printDicts("before:");
        Console.WriteLine($"Remove {activity}");
        _startTimes.Remove(activity);
        _eventIds.Remove(activity);
        printDicts("after:");
    }
    public string? TryPostEvent(Activity activity, DateTime start, DateTime end)
    {
        _eventIds.TryGetValue(activity, out string? existingId);
        Event @event = activity.ToEvent(start, end, Config.ColorFor(activity.Category));
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

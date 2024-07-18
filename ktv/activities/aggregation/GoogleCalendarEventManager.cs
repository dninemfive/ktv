using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;
using EventStartTime = (System.DateTime startTime, string eventId);

namespace d9.ktv;
public class GoogleCalendarEventManager
{
    public ActivityAggregationConfig Config { get; private set; }
    private readonly Dictionary<Activity, EventStartTime> _startedEvents = new();
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
        foreach (Activity activity in summary.Union(_startedEvents.Keys))
        {
            if (summary.TryGetValue(activity, out float percentage)
                && percentage >= Config.GoogleCalendar?.ActivityPercentageThreshold)
            {
                if(_startedEvents.TryGetValue(activity, out EventStartTime eventStartTime))
                {
                    (DateTime start, string id) = eventStartTime;
                    UpdateEvent(id, activity, start, summary.End);
                }
                else
                {
                    string? id = TryPostEvent(activity, summary.Start, summary.End);
                    if (id is not null)
                        _startedEvents.Add(activity, (summary.Start, id));
                }
            } 
            else
            {
                _startedEvents.Remove(activity);
            }
        }
    }
    public void UpdateEvent(string eventId, Activity activity, DateTime start, DateTime end)
    {
        try
        {
            _ = GoogleUtils.UpdateEvent(Config.GoogleCalendar!.Id, eventId, activity.ToEvent(start, end, Config.ColorFor(activity.Category)));
        }
        catch(Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().Name}: {e.Message}");
        }
    }
    public string? TryPostEvent(Activity activity, DateTime start, DateTime end)
    {
        try
        {
            return GoogleUtils.AddEventTo(Config.GoogleCalendar!.Id, activity.ToEvent(start, end, Config.ColorFor(activity.Category))).Id;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().Name}: {e.Message}");
            return null;
        }
    }
}

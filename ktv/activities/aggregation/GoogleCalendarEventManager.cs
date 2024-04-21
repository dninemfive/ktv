using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class GoogleCalendarEventManager
{
    public ActivityAggregationConfig Config { get; private set; }
    private static readonly Dictionary<Activity, GoogleCalendarEventInfo> _activitiesInProgress = new();
    private GoogleCalendarEventManager(ActivityAggregationConfig cfg)
        => Config = cfg;
    public static GoogleCalendarEventManager? From(ActivityAggregationConfig config)
    {
        if (config.GoogleCalendar is null)
            return null;
        return new(config);
    }
    public IEnumerable<Event> EventsFrom(ActivitySummary summary, float threshold = 0.3f)
    {
        foreach (Activity activity in summary)
        {
            float pct = summary[activity];
            if (pct < threshold)
            {
                // the activity has ended, post a final time and remove it from _activitiesInProgress
                continue;
            }
            _activitiesInProgress.TryGetValue(activity, out GoogleCalendarEventInfo? info);
            info ??= new(summary.Start, summary.End);
            info.EndTime = summary.End;
            _activitiesInProgress[activity] = info;
        }
    }
    public void PostEvent(Activity activity, GoogleCalendarEventInfo info)
    {
        Event @event = new ()
        {
            Summary = activity.Name,
            Description = activity.Category,
            Start = info.StartTime.Floor().ToEventDateTime(),
            End = info.EndTime.Floor().ToEventDateTime(),
            ColorId = Config.GoogleCalendar!.GetColorFor(activity.Category).ToColorId()
        };
        try
        {
            if (info.Id is not null)
            {
                GoogleUtils.UpdateEvent(Config.GoogleCalendar!.Id, info.Id, @event);
            }
            else
            {
                info.Id = GoogleUtils.AddEventTo(Config.GoogleCalendar!.Id, @event).Id;
            }
        } 
        catch(Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().Name}: {e.Message}");
        }
    }
}

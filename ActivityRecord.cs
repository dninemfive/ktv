using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;

namespace d9.ktv
{
    internal class ActivityRecord
    {
        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt => activities?.Select(x => x.Key)
                                               .OrderBy(x => x)
                                               .Last()
                                               .Round() ?? null;
        private readonly Dictionary<DateTime, string> activities = new();
        public IEnumerable<(DateTime timestamp, string activity)> ActivityTimestamps => activities.OrderBy(x => x.Key)
                                                                                                  .Select(x => (x.Key, x.Value));
        public IEnumerable<string> Activities => ActivityTimestamps.Select(x => x.activity);
        public DateTime Date => StartedAt.Date;
        public ActivityRecord(string? activity = null)
        {
            StartedAt = DateTime.Now;
            if (activity is not null) Log(activity);
        }
        public void Log(string activity)
        {
            activities[DateTime.Now] = activity;
        }
        public IEnumerable<string> MostCommon
        {
            get
            {
                float threshold = 0.4f;
                if (!Activities.Any())
                {
                    yield return "";
                    yield break;
                }
                Dictionary<string, int> dict = new();
                foreach (string activity in Activities)
                {
                    if (!dict.ContainsKey(activity)) dict.Add(activity, 1);
                    else dict[activity]++;
                }
                List<(string activity, float percent)> orderedActivities = dict.Keys.Select(x => (x, dict[x] / (float)activities.Count))
                                                                                    .OrderByDescending(x => x.Item2).ToList();
                List<string> result = new();
                int ctYielded = 0;
                while(!result.Any() && threshold > 0)
                {
                    foreach((string activity, float percent) in orderedActivities)
                    {
                        if (percent >= threshold)
                        {
                            yield return $"{$"{percent:p0}",4}\t{activity}";
                            ctYielded++;
                        }
                        else if (ctYielded > 0) yield break;
                    }
                    threshold -= 0.05f;
                }
            }
        }
        public bool TryMerge(ActivityRecord other, string? CalendarConfigId)
        {
            if (other is null 
            || other.Date != Date 
            || other.MostCommon != MostCommon) return false;
            if (EndedAt is null || other.EndedAt is null)
                throw new Exception("Should not merge ActivityRecords when one of their EndedAt values is null!\n"
                                 + $"\tother.EndedAt = {other.EndedAt.PrintNull()}\n"
                                 + $"\tthis.EndedAt  = {EndedAt.PrintNull()}");
            DateTime first = other.StartedAt < StartedAt ? other.StartedAt : StartedAt;
            StartedAt = first;
            foreach (KeyValuePair<DateTime, string> kvp in other.activities) activities.Add(kvp.Key, kvp.Value);
            if(CalendarConfigId is not null)
            {
                foreach(Event calendarEvent in CalendarEvents) calendarEvent.SendToCalendar(CalendarConfigId, Program.LastEventId);
            }            
            return true;
        }
        public override string ToString() => $"{StartedAt.Time(),-8}\t{(EndedAt?.Time()).PrintNull(),-8}\t{MostCommon.PrintNull()}";
        public static readonly string Header = $"{"Start",-8}\t{"End",-8}\tActivity";
        public static string AggregateFile(DateTime date) => Path.Join(Program.LogFolder, $"{date.ToString(TimeFormats.Date)}-aggregate.ktv.log");
        public string Data
        {
            get
            {
                string result = $"ActivityRecord {StartedAt:G}-{EndedAt:G} ({MostCommon}) {{\n";
                foreach((DateTime timestamp, string activity) in ActivityTimestamps)
                {
                    result += $"\t{timestamp.Time()}: {activity}\n";
                }
                return result + "}";
            }
        }
        public bool FromToday => Date == DateTime.Today;
        public IEnumerable<Event> CalendarEvents => MostCommon.Select(x => new Event()
        {
            Summary = x,
            Start = StartedAt.Round().ToEventDateTime(),
            End = EndedAt?.ToEventDateTime(),
            ColorId = Program.ColorIdFor(x)
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using d9.utl;

namespace d9.ktv
{
    internal class ActivityRecord
    {
        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt => activities?.Select(x => x.Key)
                                               .OrderBy(x => x)
                                               .Last() ?? null;
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
            _mostCommon = null;
        }
        private string? _mostCommon = null;
        public string MostCommon
        {
            get
            {
                if(_mostCommon is null)
                {
                    if (!Activities.Any()) return _mostCommon.PrintNull();
                    Dictionary<string, int> dict = new();
                    foreach (string activity in Activities)
                    {
                        if (!dict.ContainsKey(activity)) dict.Add(activity, 1);
                        else dict[activity]++;
                    }
                    IEnumerable<string> mostCommons = dict.Select(x => (x.Key, x.Value))
                                                          .Where(x => x.Value == dict.Select(x => x.Value).Max())
                                                          .Select(x => x.Key);
                    if (mostCommons.Count() == 1) _mostCommon = mostCommons.First();
                    else _mostCommon = mostCommons.OrderBy(x => x).ListNotation();
                }
                return _mostCommon;
            }
        }
        public bool TryMerge(ActivityRecord other)
        {
            if (other is null 
            || other.Date != Date 
            || other.MostCommon != MostCommon) return false;
            if (EndedAt is null || other.EndedAt is null)
                throw new Exception("Should not merge ActivityRecords when one of their EndedAt values is null!\n"
                                 + $"\tother.EndedAt = {other.EndedAt.PrintNull()}\n"
                                 + $"\tthis. EndedAt = {EndedAt.PrintNull()}");
            DateTime first = other.StartedAt < StartedAt ? other.StartedAt : StartedAt;
            StartedAt = first;
            foreach (KeyValuePair<DateTime, string> kvp in other.activities) activities.Add(kvp.Key, kvp.Value);
            // update calendar with new ending time
            return true;
        }
        public override string ToString() => $"{StartedAt.Time(),-8}\t{(EndedAt?.Ceiling().Time()).PrintNull(),-8}\t{MostCommon.PrintNull()}";
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
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal class ActivityRecord
    {
        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt { get; private set; } = null;
        private readonly List<string> activities = new();
        public IEnumerable<string> Activities
        {
            get
            {
                foreach (string activity in activities) yield return activity;
            }
        }
        public DateTime Date => StartedAt.Date;
        public ActivityRecord(string? activity = null)
        {
            StartedAt = DateTime.Now;
            if (activity is not null) Log(activity);
        }
        public void Log(string activity)
        {
            activities.Add(activity);
            EndedAt = DateTime.Now;
            _mostCommon = null;
        }
        private string? _mostCommon = null;
        public string MostCommon
        {
            get
            {
                if(_mostCommon is null)
                {
                    if (!activities.Any()) return _mostCommon.DefinitelyReadableString();
                    Dictionary<string, int> dict = new();
                    foreach (string activity in activities)
                    {
                        if (!dict.ContainsKey(activity)) dict.Add(activity, 1);
                        else dict[activity]++;
                    }
                    IEnumerable<string> mostCommons = dict.Select(x => (x.Key, x.Value))
                                                          .Where(x => x.Value == dict.Select(x => x.Value).Max())
                                                          .Select(x => x.Key);
                    if (mostCommons.Count() == 1) _mostCommon = mostCommons.First();
                    else _mostCommon = mostCommons.OrderBy(x => x).Readable();
                }
                return _mostCommon;
            }
        }
        public void Merge(ActivityRecord other)
        {
            if (other is null || other.Date != Date || other.MostCommon != MostCommon) return;
            if (EndedAt is null || other.EndedAt is null)
                throw new Exception("Should not merge ActivityRecords when one of their EndedAt values is null!\n"
                                 + $"\tother.EndedAt = {other.EndedAt.DefinitelyReadableString()}\n"
                                 + $"\tthis. EndedAt = {EndedAt.DefinitelyReadableString()}");
            DateTime first = other.StartedAt < StartedAt ? other.StartedAt : StartedAt;
            DateTime otherEA = other.EndedAt.Value, thisEA = EndedAt.Value;
            DateTime last = otherEA > thisEA ? otherEA : thisEA;

            StartedAt = first;
            EndedAt = last;
            foreach (string activity in other.Activities) activities.Add(activity);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class ActivitySummary(IEnumerable<(Activity activity, float percentage)> activities, DateTime start, DateTime end)
    : IEnumerable<Activity>
{
    public IReadOnlyDictionary<Activity, float> Activities { get; private set; } = activities.ToDictionary();
    public DateTime Start { get; private set; } = start;
    public DateTime End { get; private set; } = end;
    public float this[Activity activity] => Activities[activity];
    public IEnumerator<Activity> GetEnumerator()
        => Activities.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => Activities.Keys.GetEnumerator();
}

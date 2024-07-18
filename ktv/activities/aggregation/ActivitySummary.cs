using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class ActivitySummary(IReadOnlyDictionary<Activity, float> activities, DateTime start, DateTime end)
    : IEnumerable<Activity>
{
    public IReadOnlyDictionary<Activity, float> Activities { get; private set; } = activities;
    public DateTime Start { get; private set; } = start;
    public DateTime End { get; private set; } = end;
    public float this[Activity activity] => Activities[activity];
    public bool TryGetValue(Activity activity, [NotNullWhen(true)] out float result)
        => Activities.TryGetValue(activity, out result);
    public IEnumerator<Activity> GetEnumerator()
        => Activities.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => Activities.Keys.GetEnumerator();
}

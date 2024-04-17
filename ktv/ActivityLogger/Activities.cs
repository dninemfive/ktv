using d9.utl;

namespace d9.ktv.ActivityLogger;
public static class Activities
{
    private static readonly Dictionary<string, Activity> _activities = new();
    private static Activity GetOrMakeActivity(string name, DateTime start, DateTime end, string? description = null, bool makeEvent = false)
    {
        if (_activities.TryGetValue(name, out Activity? activity))
        {
            activity.End = end;
            return activity;
        }
        Activity result = new(name, start.Floor(Program.Args.AggregationInterval), end, description, makeEvent);
        _activities[name] = result;
        return result;
    }
    public static IEnumerable<(Activity activity, float proportion)> Between(DateTime start, DateTime end, float threshold = 0.3f)
    {
        CountingDictionary<string, int> entryCounts = new();
        HashSet<string> activeActivities = new();
        foreach (ActiveWindowLogEntry entry in WindowNameLog.EntriesBetween(start, end))
            entryCounts.Add(entry.WindowName, 1);
        int sum = entryCounts.Total;
        foreach (KeyValuePair<string, int> kvp in entryCounts.Descending())
        {

            (string entry, int value) = kvp;
            float proportion = value / (float)sum;
            yield return (GetOrMakeActivity(entry, start, end, makeEvent: proportion > threshold), proportion);
            if (proportion > threshold)
            {
                _ = activeActivities.Add(entry);
            }
        }
        foreach (KeyValuePair<string, Activity> kvp in _activities)
        {
            if (!activeActivities.Contains(kvp.Key))
                _ = _activities.Remove(kvp.Key);
        }
    }
}
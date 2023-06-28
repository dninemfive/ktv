namespace d9.ktv;
public class Activity
{
    public readonly string Name;
    public readonly string? Description;
    public string? EventId { get; private set; } = null;
    public readonly DateTime Start;
    private DateTime _end;
    public DateTime End
    {
        get => _end;
        set
        {
            _end = value;
            EventId = CalendarManager.PostOrUpdateEvent(Name, Start, End, EventId);
        }
    }
    public Activity(string name, DateTime start, DateTime end, string? description = null)
    {
        Name = name;
        Start = start;
        End = end;
        Description = description;
        //EventId = CalendarManager.PostOrUpdateEvent(name, start, end);
    }
    public override string ToString() => $"{Start} - {End}: {Name}{(!string.IsNullOrEmpty(Description) ? $" ({Description})" : "")}";
}
public static class Activities
{
    private static readonly Dictionary<string, Activity> _activities = new();
    private static Activity GetOrMakeActivity(string name, DateTime start, DateTime end, string? description = null)
    {
        if (_activities.TryGetValue(name, out Activity? activity))
            return activity;
        Activity result = new(name, start.Floor(Program.Args.AggregationInterval), end, description);
        _activities[name] = result;
        return result;
    }
    public static IEnumerable<(Activity activity, float proportion)> Between(DateTime start, DateTime end, float threshold = 0.4f)
    {
        CountingDictionary<string, int> entryCounts = new();
        HashSet<string> activeActivities = new();
        foreach (WindowNameLog.Entry entry in WindowNameLog.EntriesBetween(start, end))
            entryCounts.Add(entry.WindowName);
        int sum = entryCounts.Total;
        foreach(KeyValuePair<string, int> kvp in entryCounts.Descending())
        {
            
            (string entry, int value) = kvp;
            float proportion = value / (float)sum;
            if (proportion > threshold)
            {                
                yield return (GetOrMakeActivity(entry, start, end, $"{proportion:p2}"), proportion);
                _ = activeActivities.Add(entry);
            }                
        }
        foreach(KeyValuePair<string, Activity> kvp in _activities)
        {
            if (!activeActivities.Contains(kvp.Key))
                _ = _activities.Remove(kvp.Key);
        }
    }
}
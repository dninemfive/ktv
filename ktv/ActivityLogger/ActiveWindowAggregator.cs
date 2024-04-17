using d9.utl;
using System.Text.Json;

namespace d9.ktv.ActivityLogger;
public class ActiveWindowAggregator(TimeSpan period) : TaskScheduler
{
    public TimeSpan Period => period;
    public static IEnumerable<ActiveWindowLogEntry> EntriesBetween(DateTime start, DateTime end)
    {
        foreach(string fileName in ActiveWindowLogUtils.FileNamesFor(start, end))
        {
            foreach(string line in File.ReadAllLines(fileName))
            {
                ActiveWindowLogEntry? entry = JsonSerializer.Deserialize<ActiveWindowLogEntry>(line);
                if(entry is not null && entry.DateTime >= start)
                {
                    if(entry.DateTime <= end)
                        yield return entry;
                    if (entry.DateTime >= end)
                        break;
                }
            }
        }
    }
    public override ScheduledTask NextTask(DateTime time)
    {
        DateTime nextTime = time + Period;
        return new(nextTime, () => Aggregate(nextTime), this);
    }
    private void Aggregate(DateTime time)
    {
        IEnumerable<ActiveWindowLogEntry> entries = EntriesBetween(time, time + Period);
        // todo: set up a syntax to parse the active window process name and window name
        CountingDictionary<string, int> dict = new();
        foreach (string? s in entries.Select(x => x?.ProcessName))
            if (s is not null)
                dict.Increment(s);
        int maxCt = dict.Select(x => x.Value).Max();
        Console.WriteLine($"{DateTime.Now:g} most common processes in the last {Period.Natural()} ({maxCt} items each):");
        foreach ((string key, int value) in (IEnumerable<KeyValuePair<string, int>>)dict)
        {
            if (value == maxCt)
                Console.WriteLine($"\t{key}");
        }
    }
}

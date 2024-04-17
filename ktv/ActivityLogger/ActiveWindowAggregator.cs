using d9.utl;
using System.Text.Json;

namespace d9.ktv.ActivityLogger;
public class ActiveWindowAggregator : TaskScheduler
{
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
    private void Aggregate()
    {
        IEnumerable<ActiveWindowLogEntry> entries = File.ReadAllLines(FileName!)
                                                         .Select(x => JsonSerializer.Deserialize<ActiveWindowLogEntry>(x));
        // todo: set up a syntax to parse the active window process name and window name
        CountingDictionary<string, int> dict = new();
        foreach (string? s in entries.Select(x => x?.ProcessName))
            if (s is not null)
                dict.Increment(s);
        int maxCt = dict.Select(x => x.Value).Max();
        Console.WriteLine($"{DateTime.Now:g} most common processes in the last {LogsPerAggregation} logs ({maxCt} items each):");
        foreach ((string key, int value) in (IEnumerable<KeyValuePair<string, int>>)dict)
        {
            if (value == maxCt)
                Console.WriteLine($"\t{key}");
        }
    }
}

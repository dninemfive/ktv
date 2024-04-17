using d9.utl;
using System.Diagnostics;
using System.Text.Json;

namespace d9.ktv;
public class ActiveWindowLogger(TimeSpan logPeriod, TimeSpan aggregationPeriod) : TaskScheduler
{
    public TimeSpan LogPeriod { get; private set; } = logPeriod;
    private static int InitializeLogsPerAggregation(TimeSpan log, TimeSpan aggregation)
    {
        if (aggregation < log)
            throw new ArgumentException($"Aggregation period must be greater than log period!");
        double dividend = aggregation / log;
        if (dividend % 1 > double.Epsilon)
            Console.WriteLine($"Log period does not evenly divide aggregation period!");
        return (int)dividend;
    }
    public int LogsPerAggregation { get; private set; } = InitializeLogsPerAggregation(logPeriod, aggregationPeriod);
    public int LogsSinceLastAggregation { get; private set; } = 0;
    public string? FileName { get; private set; } = null;
    public override ScheduledTask NextTask(DateTime time)
    {
        if (++LogsSinceLastAggregation >= LogsPerAggregation)
            Aggregate();
        FileName ??= CreateNewFile(time);
        // todo: if LogPeriod divides a day evenly and time does not line up with (now % LogPeriod), align
        return new(time + LogPeriod, LogActiveWindow, this);
    }
    private void Aggregate()
    {
        IEnumerable<ActiveWindowLogEntry?> entries = File.ReadAllLines(FileName!)
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
        // dirty the file name so it is updated in NextTask
        FileName = null;
    }
    private void LogActiveWindow()
    {
        Process? activeWindowProcess = ActiveWindow.Process;
        ActiveWindowLogEntry entry = new(DateTime.Now, activeWindowProcess);
        Console.WriteLine(entry);
        File.AppendAllText(FileName!, $"{JsonSerializer.Serialize(entry)}\n");
    }
    private static string CreateNewFile(DateTime startTime)
    {
        string fileName = $"{startTime:yyyy'-'MM'-'dd' 'HH'-'mm'-'ss}.ktv.log".FileNameSafe();
        if (!File.Exists(fileName))
            File.AppendAllText(fileName, "");
        return fileName;
    }
    public override string ToString()
        => $"ActiveWindowLogger {LogPeriod}@{LogsPerAggregation}";
}
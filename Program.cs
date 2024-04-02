using d9.utl;
using d9.utl.compat;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using d9.slp;

namespace d9.ktv;

public class Program
{
    public static class Args
    {
        public static readonly TimeSpan LogInterval
                                = CommandLineArgs.TryGet(nameof(LogInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                               ?? TimeSpan.FromMinutes(0.25);
        public static readonly TimeSpan AggregationInterval
                                = CommandLineArgs.TryGet(nameof(AggregationInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                               ?? TimeSpan.FromMinutes(15);
        public static readonly string CalendarConfigPath = CommandLineArgs.TryGet(nameof(CalendarConfigPath), CommandLineArgs.Parsers.FilePath)
                               ?? "calendar config.json";
        public static readonly bool Test = CommandLineArgs.GetFlag(nameof(Test), 't');
    }
    public static DateTime LaunchTime { get; } = DateTime.Now;
    public static SortedSet<ScheduledTask> ScheduledTasks { get; } = new();
    public static List<TaskScheduler> Schedulers { get; } = new();
    public static void Main()
    {
        DateTime now = DateTime.Now;
        Schedulers.Add(new ProcessCloser(startTime: new(0, 30), 
                                         endTime:   new(10, 0),
                                         closePeriod: TimeSpan.FromMinutes(1),
                                         processesToClose:  [new(ProcessTargetType.ProcessLocation, @"C:\Program Files (x86)\Steam"),
                                                             new(ProcessTargetType.MainWindowTitle, "Minecraft")],
                                         processesToIgnore: [new(ProcessTargetType.ProcessName, "CrashHandler")]));
        Schedulers.Add(new ProcessCloser(startTime: new(0, 30),
                                         endTime:   new(7, 0),
                                         closePeriod: TimeSpan.FromMinutes(1),
                                         processesToClose: [new(ProcessTargetType.MainWindowTitle, "Visual Studio")],
                                         []));
        Schedulers.Add(new ActiveWindowLogger(TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(15)));
        Console.WriteLine($"Schedulers: {Schedulers.ListNotation()}");
        foreach(TaskScheduler scheduler in  Schedulers)
        {
            ScheduledTasks.Add(scheduler.NextTask(now));
        }
        try
        {
            while(true)
            {
                SleepUntilNext(ScheduledTasks);
                now = DateTime.Now;
                foreach (ScheduledTask task in ScheduledTasks.Where(x => x.ScheduledTime < now).ToList())
                {
                    task.Execute();
                    ScheduledTasks.Add(task.Scheduler.NextTask(task.ScheduledTime));
                    ScheduledTasks.Remove(task);
                }
            }
        }
        finally
        {

        }
    }
    private static void SleepUntil(DateTime dt)
    {
        Console.WriteLine($"SleepUntil({dt:G})");
        int delay = (int)(dt - DateTime.Now).TotalMilliseconds;
        Thread.Sleep(delay);
    }
    private static void SleepUntilNext(IEnumerable<ScheduledTask> tasks)
        => SleepUntil(tasks.Select(x => x.ScheduledTime).Min());
}
public class ScheduledTask(DateTime time, Action execute, TaskScheduler scheduler) : IComparable<ScheduledTask>
{
    public DateTime ScheduledTime { get; private set; } = time;
    public void Execute() => execute();
    public TaskScheduler Scheduler { get; private set; } = scheduler;
    public int CompareTo(ScheduledTask? other)
        => ScheduledTime.CompareTo(other?.ScheduledTime);
}
public abstract class TaskScheduler
{
    public abstract ScheduledTask NextTask(DateTime time);
}
// future optimization: merge all ProcessClosers into one which does a decision tree of what to close
public class ProcessCloser(TimeOnly startTime,
                           TimeOnly endTime,
                           TimeSpan closePeriod,
                           IEnumerable<ProcessTargeter> processesToClose,
                           IEnumerable<ProcessTargeter> processesToIgnore) : TaskScheduler
{
    public TimeOnly StartTime { get; private set; } = startTime;
    public TimeOnly EndTime { get; private set; } = endTime;
    public TimeSpan ClosePeriod { get; private set; } = closePeriod;
    public List<ProcessTargeter> ProcessesToClose { get; private set; } = processesToClose.ToList();
    public List<ProcessTargeter> ProcessesToIgnore { get; private set; } = processesToIgnore.ToList();
    public override ScheduledTask NextTask(DateTime time)
    {
        TimeOnly nextTime = TimeOnly.FromDateTime(time + ClosePeriod);
        if(nextTime < StartTime || nextTime > EndTime) nextTime = StartTime;
        DateTime nextDateTime = new(DateOnly.FromDateTime(time), nextTime);
        if (nextDateTime <= DateTime.Now)
            nextDateTime += (DateTime.Now.Date - nextDateTime.Date) + TimeSpan.FromDays(1);
        return new(nextDateTime, CloseApplicableProcesses, this);
    }
    public void CloseApplicableProcesses()
    {
        foreach(Process process in Process.GetProcesses())
        {
            if (ProcessesToClose.Any(x => x.Matches(process)) && !ProcessesToIgnore.Any(x => x.Matches(process)))
                Console.WriteLine($"Close {process.ProcessName} ({process.MainWindowTitle})");
        }
    }
    public override string ToString()
        => $"ProcessCloser {StartTime}-{EndTime}x{ClosePeriod:g}";
}
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
        foreach((string key, int value) in (IEnumerable<KeyValuePair<string, int>>)dict)
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
        ActiveWindowLogEntry entry = new(DateTime.Now, activeWindowProcess?.ProcessName, activeWindowProcess?.MainWindowTitle);
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
public class ActiveWindowLogEntry(DateTime dateTime, string? processName, string? mainWindowTitle)
{
    [JsonInclude]
    public DateTime DateTime { get; private set; } = dateTime;
    [JsonInclude]
    public string? ProcessName { get; private set; } = processName;
    [JsonInclude]
    public string? MainWindowTitle { get; private set; } = mainWindowTitle;
    public override string ToString()
        => $"{DateTime:g}\t{ProcessName.PrintNull()}\t{MainWindowTitle.PrintNull()}";
}
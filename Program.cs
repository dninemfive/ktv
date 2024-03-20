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
    public static SortedSet<ScheduledTask> ScheduledTasks { get; }
    public static List<TaskScheduler> Schedulers { get; }
    public static void Main()
    {
        DateTime now = DateTime.Now;
        foreach(TaskScheduler scheduler in  Schedulers)
        {
            ScheduledTasks.Add(scheduler.NextTask(now));
        }
        try
        {
            while(true)
            {
                SleepUntilNext(ScheduledTasks);
                foreach(ScheduledTask task in ScheduledTasks)
                {
                    now = DateTime.Now;
                    if (task.ScheduledTime < now)
                    {
                        task.Execute();
                        ScheduledTasks.Add(task.Scheduler.NextTask(task.ScheduledTime));
                        ScheduledTasks.Remove(task);
                    } 
                    else
                    {
                        break;
                    }
                }
            }
        }
        finally
        {

        }
    }
    private static void SleepUntil(DateTime dt)
    {
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
public class ProcessCloser(IEnumerable<ProcessTargeter> processesToClose, IEnumerable<ProcessTargeter> processesToIgnore) : TaskScheduler
{
    public List<ProcessTargeter> ProcessesToClose { get; private set; } = processesToClose.ToList();
    public List<ProcessTargeter> ProcessesToIgnore { get; private set; } = processesToIgnore.ToList();
    public override ScheduledTask NextTask(DateTime time)
    {
        throw new NotImplementedException();
    }
    
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
    public override ScheduledTask NextTask(DateTime time)
    {
        if (++LogsSinceLastAggregation >= LogsPerAggregation)
            Aggregate();
        return new(time + LogPeriod, LogActiveWindow, this);
    }
    public void Aggregate()
    {

    }
    public void LogActiveWindow()
    {

    }
}
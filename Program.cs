using d9.utl;
using d9.utl.compat;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

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
    public static void Main()
    {
        try
        {
            while(true)
            {
                SleepUntilNext(ScheduledTasks);
                foreach(ScheduledTask task in ScheduledTasks)
                {
                    if (DateTime.Now > task.Time)
                    {
                        ScheduledTasks.Add(task.ExecuteAndReschedule());
                        ScheduledTasks.Remove(task);
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
        => SleepUntil(tasks.Select(x => x.Time).Min());
}
public abstract class ScheduledTask(DateTime time) : IComparable<ScheduledTask>
{
    public DateTime Time = time;
    public abstract ScheduledTask ExecuteAndReschedule();
    public int CompareTo(ScheduledTask? other)
        => Time.CompareTo(other?.Time);
}
public class LogActiveWindow(DateTime time) : ScheduledTask(time)
{
    public override ScheduledTask ExecuteAndReschedule()
    {
        return new LogActiveWindow(Time + TimeSpan.FromSeconds(15));
    }
}
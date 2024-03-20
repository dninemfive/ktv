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
                        task.Execute();
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
public class ScheduledTask(DateTime time, Action execute) : IComparable<ScheduledTask>
{
    public DateTime Time = time;
    public void Execute() => execute();
    public int CompareTo(ScheduledTask? other)
        => Time.CompareTo(other?.Time);
}
public interface IScheduleable
{
    public ScheduledTask NextTask(DateTime time);
}
public class ProgramCloser : IScheduleable
{
    public TimeSpan TimeBetweenCloseAttempts;
    public TimeOnly StartTime, EndTime;
    public ScheduledTask NextTask(DateTime time)
    {
        TimeOnly now = TimeOnly.FromDateTime(time);
        if (now < StartTime)
            return new(time + (StartTime - now), ClosePrograms);
    }
    public void ClosePrograms() { }
}
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
public class ScheduledTask : IComparable<ScheduledTask>
{
    public DateTime Time;

    public int CompareTo(ScheduledTask? other)
        => Time.CompareTo(other?.Time);
    public void Execute() { }
}
public interface IScheduleable { }
[Flags]
public enum DaysOfWeek
{
    None        = 0b00000000,
    Sunday      = 0b00000001,
    Monday      = 0b00000010,
    Tuesday     = 0b00000100,
    Wednesday   = 0b00001000,
    Thursday    = 0b00010000,
    Friday      = 0b00100000,
    Saturday    = 0b01000000,
    Weekdays    = Monday | Tuesday | Wednesday | Thursday | Friday,
    Weekends    = Saturday | Sunday,
    MWF         = Monday | Wednesday | Friday,
    TuTh        = Tuesday | Thursday,
    All         = 0b01111111
}
public struct TimeRange(TimeOnly min, TimeOnly max)
{

}
public class Schedule
{
    public DaysOfWeek Days = DaysOfWeek.All;
    
}
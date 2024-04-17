using d9.ktv.ActivityLogger;
using d9.utl;

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
    public static List<ScheduledTask> ScheduledTasks { get; } = new();
    public static List<TaskScheduler> Schedulers { get; private set; } = [
        new ProcessCloser(startTime: new(0, 30),
                          endTime: new(10, 0),
                          closePeriod: TimeSpan.FromMinutes(1),
                          processesToClose:  [new(ProcessTargetType.ProcessLocation, @"C:\Program Files (x86)\Steam"),
                                              new(ProcessTargetType.MainWindowTitle, "Minecraft")],
                          processesToIgnore: [new(ProcessTargetType.ProcessName, "CrashHandler")]),
        new ProcessCloser(startTime: new(0, 30),
                          endTime: new(7, 0),
                          closePeriod: TimeSpan.FromMinutes(1),
                          processesToClose:  [new(ProcessTargetType.MainWindowTitle, "Visual Studio")],
                          processesToIgnore: []),
        new ActiveWindowLogger(TimeSpan.FromSeconds(15)),
        new ActiveWindowAggregator(TimeSpan.FromMinutes(15))
    ];
    public static void Main()
    {
        DateTime now = DateTime.Now;
        Console.WriteLine(Schedulers.MultilineListWithAlignedTitle("schedulers:"));
        foreach(TaskScheduler scheduler in  Schedulers)
            ScheduledTasks.Add(scheduler.NextTask(now));
        try
        {
            while(true)
            {
                Console.WriteLine(ScheduledTasks.OrderBy(x => x.ScheduledTime).MultilineListWithAlignedTitle("scheduled tasks:"));
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
        // Console.WriteLine($"SleepUntil({dt:G})");
        int delay = (int)(dt - DateTime.Now).TotalMilliseconds;
        Thread.Sleep(delay);
    }
    private static void SleepUntilNext(IEnumerable<ScheduledTask> tasks)
        => SleepUntil(tasks.Select(x => x.ScheduledTime).Min());
}
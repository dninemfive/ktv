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
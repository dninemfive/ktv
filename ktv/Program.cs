using d9.ktv.ActivityLogger;
using d9.utl;
using d9.utl.compat;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        public static readonly string ConfigPath = CommandLineArgs.TryGet(nameof(ConfigPath), CommandLineArgs.Parsers.FilePath)
                               ?? "config.json";
        public static readonly bool Test = CommandLineArgs.GetFlag(nameof(Test), 't');
    }
    public static DateTime LaunchTime { get; } = DateTime.Now;
    public static List<ScheduledTask> ScheduledTasks { get; } = [];
    public static List<TaskScheduler> Schedulers { get; private set; } = [];
    public static Log Log { get; private set; } = InitializeLog();
    public static void Main()
    {
        DateTime now = DateTime.Now;
        if (Config.TryLoad<KtvConfig>(Args.ConfigPath) is not KtvConfig config)
        {
            Log.WriteLine($"Could not find valid config at expected path {Path.GetFullPath(Args.ConfigPath)}!");
            return;
        }
        Schedulers = LoadSchedulers(config).ToList();
        Log.WriteLine(Schedulers.MultilineListWithAlignedTitle("schedulers:"));
        foreach(TaskScheduler scheduler in  Schedulers)
            ScheduledTasks.Add(scheduler.NextTask(now));
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
            Log.Dispose();
        }
    }
    private static Log InitializeLog()
    {
        _ = Directory.CreateDirectory("logs");
        return new(Path.Join("logs", $"{DateTime.Now.Format()}.ktv.log"), mode: Log.Mode.WriteImmediate);
    }
    public static IEnumerable<TaskScheduler> LoadSchedulers(KtvConfig config)
    {
        if(config.ActivityTracker is ActivityTrackerConfig atc)
        {
            TimeSpan logPeriod = TimeSpan.FromMinutes(atc.LogPeriodMinutes);
            yield return new ActiveWindowLogger(logPeriod);
            if(atc.AggregationConfig is ActivityAggregationConfig aac)
                yield return new ActiveWindowAggregator(aac);
        }
        if (config.ProcessClosers is List<ProcessCloserConfig> pccs)
        {
            foreach (ProcessCloserConfig pcc in pccs)
                if (pcc.ProcessesToClose is not null || pcc.ProcessesToIgnore is not null)
                    yield return new ProcessCloser(pcc);
        }
    }
    private static void SleepUntil(DateTime dt)
    {
        int delay = (int)(dt - DateTime.Now).TotalMilliseconds;
        if(delay > 0)
            Thread.Sleep(delay);
    }
    private static void SleepUntilNext(IEnumerable<ScheduledTask> tasks)
        => SleepUntil(tasks.Select(x => x.ScheduledTime).Min());
}
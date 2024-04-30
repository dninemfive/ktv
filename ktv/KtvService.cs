using d9.ktv.ActivityLogger;
using d9.utl;

namespace d9.ktv;
public class KtvService : IDisposable
{
    private bool _disposed;
    private List<TaskScheduler> _schedulers = [];
    private List<ScheduledTask> _scheduledTasks = [];
    private KtvConfig Config { get; set; }
    private Log Log { get; set; }
    public KtvService(KtvConfig config, Log log)
    {
        Config = config;
        Log = log;
    }
    public KtvService(KtvConfig config, IConsole? console = null) 
        : this(config, new Log(DateTime.Now.GenerateLogFile(), console, mode: Log.Mode.WriteImmediate)) { }
    public async Task Run()
    {
        DateTime now = DateTime.Now;
        _schedulers = LoadSchedulers(Config).ToList();
        Log.WriteLine(_schedulers.MultilineListWithAlignedTitle("schedulers:"));
        foreach (TaskScheduler scheduler in _schedulers)
        {
            scheduler.SetUp();
            _scheduledTasks.Add(scheduler.NextTask(now));
        }
        while (true)
        {
            SleepUntilNext(_scheduledTasks);
            now = DateTime.Now;
            foreach (ScheduledTask task in _scheduledTasks.Where(x => x.ScheduledTime < now).ToList())
            {
                task.Execute();
                _scheduledTasks.Add(task.Scheduler.NextTask(task.ScheduledTime));
                _scheduledTasks.Remove(task);
            }
        }
    }
    public static IEnumerable<TaskScheduler> LoadSchedulers(KtvConfig config)
    {
        if (config.ActivityTracker is ActivityTrackerConfig atc)
        {
            TimeSpan logPeriod = TimeSpan.FromMinutes(atc.LogPeriodMinutes);
            yield return new ActiveWindowLogger(logPeriod);
            if (atc.AggregationConfig is ActivityAggregationConfig aac)
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
        if (delay > 0)
            Thread.Sleep(delay);
    }
    private static void SleepUntilNext(IEnumerable<ScheduledTask> tasks)
        => SleepUntil(tasks.Select(x => x.ScheduledTime).Min());

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                Log.Dispose();
            _disposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

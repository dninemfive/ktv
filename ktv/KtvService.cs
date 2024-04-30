using d9.ktv.ActivityLogger;
using d9.utl;

namespace d9.ktv;
public class KtvService : IDisposable
{
    private bool _disposed;
    private List<TaskScheduler> _schedulers = [];
    private List<Task<TaskScheduler>> _scheduledTasks = [];
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
        while (_scheduledTasks.Any())
        {
            Task<TaskScheduler> nextCompletedTask = await Task.WhenAny(_scheduledTasks);
            _scheduledTasks.Remove(nextCompletedTask);
            TaskScheduler scheduler = await nextCompletedTask;
            _scheduledTasks.Add(scheduler.NextTask(DateTime.Now));
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

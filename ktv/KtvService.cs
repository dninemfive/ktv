using d9.ktv.ActivityLogger;
using d9.utl;

namespace d9.ktv;
public class KtvService(KtvConfig config, Log log)
{
    private List<TaskScheduler> _schedulers = LoadSchedulers(config, log).ToList();
    public IReadOnlyList<TaskScheduler> Schedulers => _schedulers;
    private readonly List<Task<TaskScheduler>> _scheduledTasks = [];
    private KtvConfig Config { get; set; } = config;
    private Log Log { get; set; } = log;
    private bool _running = false;
    public static KtvService CreateAndLog(KtvConfig config, Log log)
    {
        KtvService result = new(config, log);
        log.WriteLine(result._schedulers.MultilineListWithAlignedTitle("schedulers:"));
        return result;
    }
    public async Task Run()
    {
        _running = !_running ? true : throw new Exception("Attempted to run a KtvService which was already running!");
        DateTime now = DateTime.Now;
        await Task.Delay(1000000);
        foreach (TaskScheduler scheduler in _schedulers)
        {
            scheduler.SetUp();
            _scheduledTasks.Add(scheduler.NextTask(now));
        }
        try
        {
            while (_scheduledTasks.Any())
            {
                Task<TaskScheduler> nextCompletedTask = await Task.WhenAny(_scheduledTasks);
                _scheduledTasks.Remove(nextCompletedTask);
                TaskScheduler scheduler = await nextCompletedTask;
                _scheduledTasks.Add(scheduler.NextTask(DateTime.Now));
            }
        }
        finally
        {
            await Log.WriteLine("ktv shutting down...");
            Log.Dispose();
        }
    }
    public static IEnumerable<TaskScheduler> LoadSchedulers(KtvConfig config, Log log)
    {
        if (config.ActivityTracker is ActivityTrackerConfig atc)
        {
            TimeSpan logPeriod = TimeSpan.FromMinutes(atc.LogPeriodMinutes);
            yield return new ActiveWindowLogger(logPeriod, log);
            if (atc.AggregationConfig is ActivityAggregationConfig aac)
                yield return new ActiveWindowAggregator(aac, config.ProcessMatchModeImplementation, log);
        }
        if (config.ProcessClosers is List<ProcessCloserConfig> pccs)
        {
            foreach (ProcessCloserConfig pcc in pccs)
                if (pcc.ProcessesToClose is not null || pcc.ProcessesToIgnore is not null)
                    yield return new ProcessCloser(pcc, config.ProcessMatchModeImplementation, log);
        }
    }
}

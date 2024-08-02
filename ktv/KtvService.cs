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
    public static async Task<KtvService> CreateAndLog(KtvConfig config, Log log)
    {
        await log.WriteLine("1");
        KtvService result = new(config, log);
        await log.WriteLine(result.PrettyPrint());
        await log.WriteLine("2");
        // await log.WriteLine(result._schedulers.MultilineListWithAlignedTitle("schedulers:"));
        return result;
    }
    public async Task Run()
    {
        await Log.WriteLine("Run()");
        _running = !_running ? true : throw new Exception("Attempted to run a KtvService which was already running!");
        DateTime now = DateTime.Now;
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
            log.WriteLine($"Loading activity tracker...");
            TimeSpan logPeriod = TimeSpan.FromMinutes(atc.LogPeriodMinutes);
            log.WriteLine($"\tyielding activewindowlogger...");
            yield return new ActiveWindowLogger(logPeriod, log);
            log.WriteLine($"\tyielding aggregator...");
            if (atc.AggregationConfig is ActivityAggregationConfig aac)
            {
                log.WriteLine($"\t\t{aac}");
                yield return new ActiveWindowAggregator(aac, config.ProcessMatchModeImplementation, log);
            }
            log.WriteLine($"...done");
        }
        if (config.ProcessClosers is List<ProcessCloserConfig> pccs)
        {
            log.WriteLine($"loading process closers");
            foreach (ProcessCloserConfig pcc in pccs)
                if (pcc.ProcessesToClose is not null || pcc.ProcessesToIgnore is not null)
                    yield return new ProcessCloser(pcc, config.ProcessMatchModeImplementation, log);
        }
        log.WriteLine($"...done");
    }
}

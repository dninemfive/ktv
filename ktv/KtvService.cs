using d9.ktv.ActivityLogger;
using d9.utl;

namespace d9.ktv;
public class KtvService(KtvConfig config, Progress<string> progress)
{
    private List<TaskScheduler> _schedulers = LoadSchedulers(progress, config).ToList();
    public IReadOnlyList<TaskScheduler> Schedulers => _schedulers;
    private readonly List<Task<TaskScheduler>> _scheduledTasks = [];
    private KtvConfig Config { get; set; } = config;
    private Progress<string> Progress { get; set; } = progress;
    private bool _running = false;
    public static KtvService CreateAndLog(KtvConfig config, Progress<string> progress)
    {
        KtvService result = new(config, progress);
        progress.Report(result._schedulers.MultilineListWithAlignedTitle("schedulers:"));
        return result;
    }
    public async Task Run()
    {
        _running = !_running ? true : throw new Exception("Attempted to run a KtvService which was already running!");
        DateTime now = DateTime.Now;
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
    public static IEnumerable<TaskScheduler> LoadSchedulers(Progress<string> progress, KtvConfig config)
    {
        if (config.ActivityTracker is ActivityTrackerConfig atc)
        {
            TimeSpan logPeriod = TimeSpan.FromMinutes(atc.LogPeriodMinutes);
            yield return new ActiveWindowLogger(progress, logPeriod);
            if (atc.AggregationConfig is ActivityAggregationConfig aac)
                yield return new ActiveWindowAggregator(progress, aac, config.ProcessMatchModeImplementation);
        }
        if (config.ProcessClosers is List<ProcessCloserConfig> pccs)
        {
            foreach (ProcessCloserConfig pcc in pccs)
                if (pcc.ProcessesToClose is not null || pcc.ProcessesToIgnore is not null)
                    yield return new ProcessCloser(progress, pcc, config.ProcessMatchModeImplementation);
        }
    }
}

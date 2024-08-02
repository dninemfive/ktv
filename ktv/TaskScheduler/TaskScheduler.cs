using d9.utl;

namespace d9.ktv;
public abstract class TaskScheduler(Log log, Progress<TimeFraction?>? updateProgress = null)
{
    public Log Log { get; set; } = log;
    public Progress<TimeFraction?> UpdateProgress { get; set; } = updateProgress ?? new();
    public virtual void SetUp() { }
    public abstract Task<TaskScheduler> NextTask(DateTime time);
    public void Report(object? obj)
        => Log.WriteLine(obj);
}
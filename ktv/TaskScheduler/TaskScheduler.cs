namespace d9.ktv;
public abstract class TaskScheduler(Progress<string> logProgress, Progress<TimeFraction?>? updateProgress = null)
{
    public Progress<string> Progress { get; set; } = logProgress;
    public Progress<TimeFraction?> UpdateProgress { get; set; } = updateProgress ?? new();
    public virtual void SetUp() { }
    public abstract Task<TaskScheduler> NextTask(DateTime time);
    public void Report(object? obj)
        => Progress.Report($"{obj}");
}
namespace d9.ktv;
public abstract class TaskScheduler(Progress<string> progress)
{
    public Progress<string> Progress { get; set; } = progress;
    public virtual void SetUp() { }
    public abstract Task<TaskScheduler> NextTask(DateTime time);
    public void Report(object? obj)
        => Progress.Report($"{obj}");
}
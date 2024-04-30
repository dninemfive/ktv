namespace d9.ktv;
/// <summary>
/// Abstracts the pattern of a task scheduler which schedules a task at a fixed interval which
/// tries to align to the current day.
/// </summary>
/// <param name="period"></param>
public abstract class FixedPeriodTaskScheduler(Progress<string> progress, TimeSpan period) : TaskScheduler(progress)
{
    public TimeSpan Period => period;
    public override async Task<TaskScheduler> NextTask(DateTime time)
    {
        await Task.Delay(time.NextDayAlignedTime(Period) - time);
        return NextTaskInternal(time.NextDayAlignedTime(Period));
    }
    protected abstract TaskScheduler NextTaskInternal(DateTime time);
    public override string ToString()
        => $"{GetType().Name}({Period})";
}

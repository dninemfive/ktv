namespace d9.ktv;
/// <summary>
/// Abstracts the pattern of a task scheduler which schedules a task at a fixed interval which
/// tries to align to the current day.
/// </summary>
/// <param name="period"></param>
public abstract class FixedPeriodTaskScheduler(TimeSpan period) : TaskScheduler
{
    public TimeSpan Period => period;
    public override ScheduledTask NextTask(DateTime time)
        => NextTaskInternal(time.NextDayAlignedTime(Period));
    protected abstract ScheduledTask NextTaskInternal(DateTime time);
    public override string ToString()
        => $"{GetType().Name}({Period})";
}

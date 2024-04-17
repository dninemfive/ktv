namespace d9.ktv;
public abstract class FixedPeriodTaskScheduler(TimeSpan period) : TaskScheduler
{
    public TimeSpan Period => period;
    public override ScheduledTask NextTask(DateTime time)
        => NextTaskInternal(time.NextDayAlignedTime(Period));
    protected abstract ScheduledTask NextTaskInternal(DateTime time);
}

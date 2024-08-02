using d9.utl;

namespace d9.ktv;
/// <summary>
/// Abstracts the pattern of a task scheduler which schedules a task at a fixed interval which
/// tries to align to the current day.
/// </summary>
/// <param name="period"></param>
public abstract class FixedPeriodTaskScheduler(TimeSpan period, Log log, Progress<TimeFraction?> timeProgress)
    : TaskScheduler(log, timeProgress)
{
    public FixedPeriodTaskScheduler(TimeSpan period, Log log) : this(period, log, new()) { }
    public TimeSpan Period => period;
    public override async Task<TaskScheduler> NextTask(DateTime time)
    {
        TimeSpan delay = time.NextDayAlignedTime(Period) - time;
        ((IProgress<TimeFraction?>)UpdateProgress).Report(new(delay, Period));
        await Task.Delay(delay);
        return NextTaskInternal(time.NextDayAlignedTime(Period));
    }
    protected abstract TaskScheduler NextTaskInternal(DateTime time);
    public override string ToString()
        => $"{GetType().Name}({Period})";
}

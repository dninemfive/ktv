namespace d9.ktv;
public class ScheduledTask(DateTime time, Action execute, TaskScheduler scheduler) : IComparable<ScheduledTask>
{
    public DateTime ScheduledTime { get; private set; } = time;
    public void Execute() => execute();
    public TaskScheduler Scheduler { get; private set; } = scheduler;
    public int CompareTo(ScheduledTask? other)
        => ScheduledTime.CompareTo(other?.ScheduledTime);
    public override string ToString()
        => $"{GetType().Name} @ {ScheduledTime} ({Scheduler.GetType().Name})";
}
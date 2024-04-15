namespace d9.ktv;
public abstract class TaskScheduler
{
    public abstract ScheduledTask NextTask(DateTime time);
}
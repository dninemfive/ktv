namespace d9.ktv;
public abstract class TaskScheduler
{
    public virtual void SetUp() { }
    public abstract ScheduledTask NextTask(DateTime time);
}
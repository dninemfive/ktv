using d9.utl;

namespace d9.ktv;
public abstract class TaskScheduler(Log? log = null)
{
    public Log? Log { get; private set; } = log;
    public virtual void SetUp() { }
    public abstract Task<TaskScheduler> NextTask(DateTime time);
}
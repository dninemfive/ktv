using d9.ktv.ActivityLogger;
using d9.utl;
using System.Collections;

namespace d9.ktv;
public class TaskManager(IEnumerable<TaskScheduler> schedulers)
    : IEnumerable<TaskScheduler>
{
    private readonly List<TaskScheduler> _schedulers = [.. schedulers];
    public IEnumerator<TaskScheduler> GetEnumerator()
        => ((IEnumerable<TaskScheduler>)_schedulers).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_schedulers).GetEnumerator();
    private bool _running = false;
    private readonly List<Task<TaskScheduler>> _scheduledTasks = [];
    public IEnumerable<Task<TaskScheduler>> ScheduledTasks => _scheduledTasks;
    public async Task Run()
    {
        _running = !_running ? true : throw new Exception("Attempted to run a TaskManager which was already running!");
        DateTime now = DateTime.Now;
        foreach (TaskScheduler scheduler in _schedulers)
        {
            scheduler.SetUp();
            _scheduledTasks.Add(scheduler.NextTask(now));
        }
        while (_scheduledTasks.Any())
        {
            Task<TaskScheduler> nextCompletedTask = await Task.WhenAny(_scheduledTasks);
            _scheduledTasks.Remove(nextCompletedTask);
            TaskScheduler scheduler = await nextCompletedTask;
            _scheduledTasks.Add(scheduler.NextTask(DateTime.Now));
        }
    }
    public T? First<T>()
        where T : TaskScheduler
        => _schedulers.Any() ? _schedulers.OfType<T>().First() : null;
}

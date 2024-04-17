using System.Diagnostics;

namespace d9.ktv;
// future optimization: merge all ProcessClosers into one which does a decision tree of what to close
public class ProcessCloser(TimeOnly startTime,
                           TimeOnly endTime,
                           TimeSpan closePeriod,
                           IEnumerable<ProcessTargeter> processesToClose,
                           IEnumerable<ProcessTargeter> processesToIgnore) : TaskScheduler
{
    public TimeOnly StartTime { get; private set; } = startTime;
    public TimeOnly EndTime { get; private set; } = endTime;
    public TimeSpan ClosePeriod { get; private set; } = closePeriod;
    public List<ProcessTargeter> ProcessesToClose { get; private set; } = processesToClose.ToList();
    public List<ProcessTargeter> ProcessesToIgnore { get; private set; } = processesToIgnore.ToList();
    public override ScheduledTask NextTask(DateTime time)
    {
        TimeOnly nextTime = TimeOnly.FromDateTime(time + ClosePeriod);
        if (nextTime < StartTime || nextTime > EndTime)
            nextTime = StartTime;
        DateTime nextDateTime = new(DateOnly.FromDateTime(time), nextTime);
        if (nextDateTime <= DateTime.Now)
            nextDateTime += (DateTime.Now.Date - nextDateTime.Date) + TimeSpan.FromDays(1);
        return new(nextDateTime, CloseApplicableProcesses, this);
    }
    public void CloseApplicableProcesses()
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (ProcessesToClose.Any(x => x.Matches(process)) && !ProcessesToIgnore.Any(x => x.Matches(process)))
                Console.WriteLine($"Close {process.ProcessName} ({process.MainWindowTitle})");
        }
    }
    public override string ToString()
        => $"ProcessCloser {StartTime}-{EndTime}x{ClosePeriod:g}";
}
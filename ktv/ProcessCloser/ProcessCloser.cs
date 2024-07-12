using d9.utl;
using System.Diagnostics;

namespace d9.ktv;
// future optimization: merge all ProcessClosers into one which does a decision tree of what to close
public class ProcessCloser(Progress<string> progress, ProcessCloserConfig config) : TaskScheduler(progress)
{
    public TimeConstraint? TimeConstraint = config.TimeConstraint;
    public TimeSpan ClosePeriod { get; private set; } = TimeSpan.FromMinutes(config.PeriodMinutes);
    public List<ProcessMatcherDef>? ProcessesToClose { get; private set; } = config.ProcessesToClose;
    public List<ProcessMatcherDef>? ProcessesToIgnore { get; private set; } = config.ProcessesToIgnore;
    public override async Task<TaskScheduler> NextTask(DateTime time)
    {
        await Task.Delay(NextDateTime(time) - DateTime.Now);
        CloseApplicableProcesses();
        return this;
    }
    public DateTime NextDateTime(DateTime time)
    {
        TimeOnly nextTime = TimeOnly.FromDateTime(time + ClosePeriod);
        DateTime nextDateTime = new(DateOnly.FromDateTime(time), nextTime);
        if (TimeConstraint is not null)
            nextDateTime = TimeConstraint.NextMatchingDateTime(nextDateTime);
        if (nextDateTime <= DateTime.Now)
            nextDateTime += (DateTime.Now.Date - nextDateTime.Date) + TimeSpan.FromDays(1);
        return nextDateTime;
    }
    public void CloseApplicableProcesses()
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (!ProcessesToIgnore.IsMatch(process) && ProcessesToClose.IsMatch(process))
            {
                Report($"Closing {process.FullInfo()}");
                process.Close();
            }
        }
    }
    public override string ToString()
        => $"ProcessCloser({TimeConstraint.PrintNull()}, {ClosePeriod:g})";
}
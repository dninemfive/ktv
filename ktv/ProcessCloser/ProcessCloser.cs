using d9.utl;
using System.Diagnostics;

namespace d9.ktv;
// future optimization: merge all ProcessClosers into one which does a decision tree of what to close
public class ProcessCloser(Progress<string> progress, ProcessCloserConfig config, ProcessMatchModeImplementation pmmi) : TaskScheduler(progress)
{
    public TimeConstraint? TimeConstraint = config.TimeConstraint;
    public TimeSpan ClosePeriod { get; private set; } = TimeSpan.FromMinutes(config.PeriodMinutes);
    public List<ProcessMatcherDef>? ProcessesToClose { get; private set; } = config.ProcessesToClose;
    public List<ProcessMatcherDef>? ProcessesToIgnore { get; private set; } = config.ProcessesToIgnore;
    public ProcessMatchModeImplementation ProcessMatchModeImplementation = pmmi;
    public override async Task<TaskScheduler> NextTask(DateTime time)
    {
        await Task.Delay(NextDateTime(time) - DateTime.Now);
        CloseMatchingProcesses();
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
    public bool ShouldClose(Process? p, DateTime dt)
    {
        if (ProcessesToClose is null && ProcessesToIgnore is null)
        {
            // never close all processes
            // maybe include an override switch but lol
            return false;
        }
        if (!(TimeConstraint?.Matches(dt) ?? false))
            return false;
        return !ProcessMatchModeImplementation.AnyMatch(ProcessesToIgnore, p) && ProcessMatchModeImplementation.AnyMatch(ProcessesToClose, p);
    }
    public void CloseMatchingProcesses()
    {
        List<string> closedProcesses = new();
        foreach (Process process in Process.GetProcesses())
        {
            if (!ProcessMatchModeImplementation.AnyMatch(ProcessesToIgnore, process) 
                && ProcessMatchModeImplementation.AnyMatch(ProcessesToClose, process, out List<ProcessMatcherDef> matches))
            {
                closedProcesses.Add($"\t{process.FullInfo()}\n\t\tMatching defs: {matches.ListNotation()}");
                process.Close();
            }
        }
        if(closedProcesses.Any())
        {
            Report($"{this} closed the following processes:");
            foreach (string s in closedProcesses)
                Report(s);
        }
    }
    public override string ToString()
        => $"ProcessCloser({TimeConstraint.PrintNull()}, {ClosePeriod:g})";
}
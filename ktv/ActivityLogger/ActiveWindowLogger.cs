using d9.utl;
using System.Diagnostics;
using System.Text.Json;

namespace d9.ktv;
public class ActiveWindowLogger(TimeSpan logPeriod) : TaskScheduler
{
    public TimeSpan LogPeriod { get; private set; } = logPeriod;
    public int LogsSinceLastAggregation { get; private set; } = 0;
    public override ScheduledTask NextTask(DateTime time)
    {
        string fileName = ActiveWindowLogUtils.FileNameFor(time);
        // todo: if LogPeriod divides a day evenly and time does not line up with (now % LogPeriod), align
        return new(time + LogPeriod, () => LogActiveWindow(fileName), this);
    }
    private void LogActiveWindow(string fileName)
    {
        Process? activeWindowProcess = ForegroundWindow.Process;
        ActiveWindowLogEntry entry = new(DateTime.Now, activeWindowProcess);
        Console.WriteLine(entry);
        File.AppendAllText(fileName, $"{JsonSerializer.Serialize(entry)}\n");
    }
    public override string ToString()
        => $"ActiveWindowLogger({LogPeriod})";
}
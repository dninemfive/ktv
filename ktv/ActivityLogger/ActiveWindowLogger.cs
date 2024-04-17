using d9.utl;
using System.Diagnostics;
using System.Text.Json;

namespace d9.ktv;
public class ActiveWindowLogger(TimeSpan period) : TaskScheduler
{
    public TimeSpan Period { get; private set; } = period;
    public int LogsSinceLastAggregation { get; private set; } = 0;
    public override ScheduledTask NextTask(DateTime time)
    {
        string fileName = ActiveWindowLogUtils.FileNameFor(time);
        return new(time.NextDayAlignedTime(Period), () => LogActiveWindow(fileName), this);
    }
    private void LogActiveWindow(string fileName)
    {
        Process? activeWindowProcess = ForegroundWindow.Process;
        ActiveWindowLogEntry entry = new(DateTime.Now, activeWindowProcess);
        Console.WriteLine(entry);
        File.AppendAllText(fileName, $"{JsonSerializer.Serialize(entry)}\n");
    }
    public override string ToString()
        => $"ActiveWindowLogger({Period})";
}
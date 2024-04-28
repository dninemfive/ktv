using System.Diagnostics;
using System.Text.Json;

namespace d9.ktv;
public class ActiveWindowLogger(TimeSpan period) : FixedPeriodTaskScheduler(period)
{
    public override void SetUp()
    {
        _ = Directory.CreateDirectory(Path.Join("logs", "ktv"));
    }
    protected override ScheduledTask NextTaskInternal(DateTime time)
    {
        string fileName = ActiveWindowLogUtils.FileNameFor(time);
        return new(time, () => LogActiveWindow(fileName), this);
    }
    private static void LogActiveWindow(string fileName)
    {
        Process? activeWindowProcess = ForegroundWindow.Process;
        ActiveWindowLogEntry entry = new(DateTime.Now, activeWindowProcess);
        File.AppendAllText(fileName, $"{JsonSerializer.Serialize(entry)}\n");
    }
    public override string ToString()
        => $"{typeof(ActiveWindowLogger).Name}({Period})";
}
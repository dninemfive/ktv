using System.Diagnostics;
using System.Text.Json;

namespace d9.ktv;
public class ActiveWindowLogger(Progress<string> progress, TimeSpan period) : FixedPeriodTaskScheduler(progress, period)
{
    public override void SetUp()
    {
        _ = Directory.CreateDirectory(Path.Join("logs", "ktv"));
    }
    protected override TaskScheduler NextTaskInternal(DateTime time)
    {
        LogActiveWindow(ActiveWindowLogUtils.FileNameFor(time));
        return this;
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
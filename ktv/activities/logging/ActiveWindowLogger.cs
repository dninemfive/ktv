using d9.utl;
using System.Diagnostics;
using System.Text.Json;

namespace d9.ktv;
public class ActiveWindowLogger(string logFolderPath, TimeSpan period, Log log) : FixedPeriodTaskScheduler(period, log)
{
    public static TimeSpan FileDuration => TimeSpan.FromMinutes(15);
    public string LogFolderPath => logFolderPath;
    public override void SetUp()
    {
        _ = Directory.CreateDirectory(LogFolderPath);
    }
    protected override TaskScheduler NextTaskInternal(DateTime time)
    {
        LogActiveWindow(FileNameFor(time));
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
    public static string FileNameFor(DateTime time)
    {
        time = time.Floor(TimeSpan.FromMinutes(15));
        string fileName = Path.Join("logs", "ktv", $"{time.Format()}.activewindow.log");
        if (!File.Exists(fileName))
            File.AppendAllText(fileName, "");
        return fileName;
    }
    public static IEnumerable<string> FileNamesFor(DateTime start, DateTime end)
    {
        DateTime cur = start;
        while (cur < end)
        {
            yield return FileNameFor(cur);
            cur += FileDuration;
        }
    }
}
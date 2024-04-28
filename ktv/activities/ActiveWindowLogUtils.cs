using d9.utl;

namespace d9.ktv;
public static class ActiveWindowLogUtils
{
    public static TimeSpan FileDuration => TimeSpan.FromMinutes(15);
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
        while(cur < end)
        {
            yield return FileNameFor(cur);
            cur += FileDuration;
        }
    }
}

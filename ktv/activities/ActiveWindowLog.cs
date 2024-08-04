using d9.utl;

namespace d9.ktv;
public class ActiveWindowLog(string baseFolder, TimeSpan? fileDuration = null)
{
    public readonly TimeSpan FileDuration = fileDuration ?? TimeSpan.FromMinutes(15);
    public readonly string FolderPath = Path.Join(baseFolder);
    public string FileNameFor(DateTime time)
    {
        time = time.Floor(TimeSpan.FromMinutes(15));
        string fileName = Path.Join(FolderPath, $"{time.Format()}.activewindow.log");
        if (!File.Exists(fileName))
            File.AppendAllText(fileName, "");
        return fileName;
    }
    public IEnumerable<string> FileNamesFor(DateTime start, DateTime end)
    {
        DateTime cur = start;
        while (cur < end)
        {
            yield return FileNameFor(cur);
            cur += FileDuration;
        }
    }
}

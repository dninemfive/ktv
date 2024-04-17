using d9.utl;
using System.Text.Json;

namespace d9.ktv;
internal static class FileManager
{    
    public static readonly string LogFolder = Path.Join(Config.BaseFolderPath, "logs");   
    static FileManager()
    {
        foreach(LogFile folder in Enum.GetValues(typeof(LogFile)))
        {
            _ = Directory.CreateDirectory(folder.FolderPath());
        }
    }
    private enum LogFile { Raw, Aggregate }
    private static string Folder(this LogFile folder) => folder.ToString().ToLower();
    private static string FolderPath(this LogFile file) => Path.Join(LogFolder, file.Folder());
    private static string Extension(this LogFile file) => $"{file.Folder()[0..3]}.ktv.log";
    private static string FilePath(this LogFile file, DateTime? date = null) 
        => Path.Join(file.FolderPath(), $"{(date ?? DateTime.Today).ToString(TimeFormats.Date)}.{file.Extension()}");
    private static IEnumerable<string> Load(LogFile file, DateTime? date = null)
    {
        string path = file.FilePath(date);
        if(!File.Exists(path))
        {
            File.WriteAllText(path, "");
            yield break;
        }
        foreach (string s in File.ReadAllLines(path))
            yield return s;
    }
    private static void AppendObj(object obj, LogFile file, DateTime? date = null) 
        => File.AppendAllText(file.FilePath(date), $"{JsonSerializer.Serialize(obj)}\n");
}
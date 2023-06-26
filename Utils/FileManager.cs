using d9.utl;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public enum LogFile { Raw, Aggregate }
    public static string Folder(this LogFile folder)
    {
        if (folder is not LogFile.Raw or LogFile.Aggregate)
            throw new Exception($"{folder} is not a valid FileManager type. Valid values are Raw or Aggregate.");
        return folder.ToString().ToLower();
    }
    public static string FolderPath(this LogFile file) => Path.Join(LogFolder, file.Folder());
    public static string Extension(this LogFile file) => $"{file.Folder()[0..3]}.ktv.log";
    public static string FilePath(this LogFile file, DateTime? date = null) 
        => Path.Join(file.FolderPath(), $"{(date ?? DateTime.Today).ToString(TimeFormats.Date)}.{file.Extension()}");
    public static IEnumerable<string> Load(LogFile file, DateTime? date = null)
    {
        string path = file.FilePath(date);
        if(!File.Exists(path))
        {
            _ = File.Create(path);
            yield break;
        }
        foreach (string s in File.ReadAllLines(path))
            yield return s;
    }
    public static void Append(LogFile file, object obj) => File.AppendAllText(file.FilePath(), JsonSerializer.Serialize(obj));
    public static IEnumerable<WindowNameLog.Entry> LoadEntries(DateTime? date = null) 
        => Load(LogFile.Raw, date)
            .Select(x => JsonSerializer
            .Deserialize<WindowNameLog.Entry>(x))
            .Where(x => x is not null)!;
}
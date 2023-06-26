﻿using d9.utl;
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
    private enum LogFile { Raw, Aggregate }
    private static string Folder(this LogFile folder)
    {
        if (folder is not LogFile.Raw or LogFile.Aggregate)
            throw new Exception($"{folder} is not a valid FileManager type. Valid values are Raw or Aggregate.");
        return folder.ToString().ToLower();
    }
    private static string FolderPath(this LogFile file) => Path.Join(LogFolder, file.Folder());
    private static string Extension(this LogFile file) => $"{file.Folder()[0..3]}.ktv.log";
    private static string FilePath(this LogFile file, DateTime? date = null) 
        => Path.Join(file.FolderPath(), $"{(date ?? DateTime.Today).ToString(TimeFormats.Date)}.{file.Extension()}");
    private static IEnumerable<string> Load(LogFile file, DateTime? date = null)
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
    private static void AppendObj(object obj, LogFile file, DateTime? date = null) 
        => File.AppendAllText(file.FilePath(date), JsonSerializer.Serialize(obj));    
    public static IEnumerable<WindowNameLog.Entry> LoadEntries(DateTime? date = null) 
        => Load(LogFile.Raw, date)
            .Select(x => JsonSerializer.Deserialize<WindowNameLog.Entry>(x))
            .Where(x => x is not null)!;
    public static void Append(WindowNameLog.Entry entry, DateTime? date = null) => AppendObj(entry, LogFile.Raw, date);
}
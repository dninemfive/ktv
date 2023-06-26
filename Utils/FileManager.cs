using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
internal static class FileManager
{
    
    public static readonly string LogFolder = Path.Join(Config.BaseFolderPath, "logs");
    public static readonly string AggregateFolder = Path.Join(LogFolder, "aggregate");
    public static string AggregateFile => Path.Join(AggregateFolder, $"{LaunchedOn.ToString(TimeFormats.Date)}.agg.ktv.log");
    public static readonly string RawFolder = Path.Join(LogFolder, "raw");
    public static readonly string RawPath = Path.Join(RawFolder, $"{DateTime.Now.Format(TimeFormats.DateTime24H)}.raw.ktv.log");
    static FileManager()
    {
        foreach(string path in new[] { LogFolder, AggregateFolder, RawFolder })
        {
            _ = Directory.CreateDirectory(path);
        }
    }
    public static string LogTo(string existingFile)
    {
        // if existing file has 1000 or more lines, create new file
    }
}

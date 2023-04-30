using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using d9.utl;

namespace d9.ktv
{
    public static class ConsoleArgs
    {
        public static readonly TimeSpan Delay 
            = CommandLineArgs.TryGet(nameof(Delay), CommandLineArgs.Parsers.TimeSpan) 
              ?? TimeSpan.FromSeconds(5);
        public static readonly TimeSpan LogInterval
            = CommandLineArgs.TryGet(nameof(LogInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
              ?? TimeSpan.FromMinutes(0.5);
        public static readonly TimeSpan? Duration = CommandLineArgs.TryGet(nameof(Duration), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes));
        public static readonly TimeSpan AggregationInterval 
            = CommandLineArgs.TryGet(nameof(AggregationInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
              ?? TimeSpan.FromMinutes(15);
        public static readonly DateTime StartAt 
            = CommandLineArgs.TryGet(nameof(StartAt), CommandLineArgs.Parsers.DateTime) 
              ?? DateTime.Now.Ceiling(AggregationInterval);
        public static readonly string LogFolder = Path.Join(Config.BaseFolderPath, "logs");
        public static readonly string LogPath = Path.Join(LogFolder, $"{DateTime.Now.Format(TimeFormats.DateTime24H)}.ktv.log");
        public static void Init()
        {
            Console.WriteLine("Starting ktv...");
            if (LogPath is not null)
            {                
                if (!Directory.Exists(LogFolder)) Directory.CreateDirectory(LogFolder);
            }
            Console.WriteLine($"Will log active window every {LogInterval:g} starting in {Delay:g}.");
            Console.WriteLine($"Will aggregate activity every {AggregationInterval:g} starting at {StartAt.Time()}.");
            Console.WriteLine($"Log folder is {LogFolder.Replace(@"\","/")}.");
            if (Duration is not null) Console.WriteLine($"Will continue for {Duration:g}.");
            Thread.Sleep(Delay);
        }
    }
}

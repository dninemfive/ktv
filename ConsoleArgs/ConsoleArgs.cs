using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    public static class ConsoleArgs
    {
        public static TimeSpan Delay => TimeSpan.FromSeconds(delay);
        private static int delay = 5;
        public static TimeSpan LogInterval => TimeSpan.FromMinutes(logInterval);
        private static float logInterval = 0.25f;
        public static TimeSpan? Duration => duration <= 0 ? null : TimeSpan.FromMinutes(duration);
        private static float duration = -1;
        public static TimeSpan AggregationInterval => TimeSpan.FromMinutes(aggregationInterval);
        private static float aggregationInterval = 15;
        public static DateTime StartAt => startAt;
        private static DateTime startAt = DateTime.MinValue;
        public static string LogFolder => logFolder;
        private static string logFolder = string.Empty;
        public static string LogPath { get; private set; } = string.Empty;
        public static bool Debug => debug;
        private static bool debug = false;
        public static void Init(string[] args)
        {
            Console.WriteLine("Starting ktv...");
            foreach (string arg in args)
            {
                ConsoleArg carg = new(arg);
                carg.TrySet(nameof(logInterval), ref logInterval, ConsoleArgParsers.Float);
                carg.TrySet(nameof(duration), ref duration, ConsoleArgParsers.Float);
                carg.TrySet(nameof(aggregationInterval), ref aggregationInterval, ConsoleArgParsers.Float);
                carg.TrySet(nameof(delay), ref delay, ConsoleArgParsers.Int);
                carg.TrySet(nameof(startAt), ref startAt, ConsoleArgParsers.DateTime);
                carg.TrySet(nameof(logFolder), ref logFolder, ConsoleArgParsers.DirectoryPath);
                carg.TrySet(nameof(debug), ref debug, ConsoleArgParsers.Bool);
            }
            #region defaults
            if (string.IsNullOrEmpty(logFolder)) logFolder = Path.GetFullPath("logs/");
            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
            if (startAt < DateTime.Now) startAt = DateTime.Now.Ceiling(AggregationInterval);
            LogPath = Path.Join(LogFolder, $"{DateTime.Now.Format(TimeFormats.DateTime24H!)}.ktv.log");
            #endregion defaults
            Console.WriteLine($"Will log active window every {LogInterval:g} starting in {Delay:g}.");
            Console.WriteLine($"Will aggregate activity every {AggregationInterval:g} starting at {StartAt.Time()}.");
            Console.WriteLine($"Log folder is {logFolder.Replace(@"\","/")}.");
            if (Duration is not null) Console.WriteLine($"Will continue for {Duration:g}.");
            Thread.Sleep(Delay);
        }
    }
}

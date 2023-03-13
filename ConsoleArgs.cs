using System;
using System.Collections.Generic;
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
        private static string logFolder = "logs/";
        public static void Init(string[] args)
        {
            Console.WriteLine("Starting ktv...");
            foreach (string arg in args)
            {
                ConsoleArg carg = new(arg);
                carg.TrySet(nameof(logInterval), ref logInterval, ConsoleArg.Parsers.Float);
                carg.TrySet(nameof(duration), ref duration, ConsoleArg.Parsers.Float);
                carg.TrySet(nameof(aggregationInterval), ref aggregationInterval, ConsoleArg.Parsers.Float);
                carg.TrySet(nameof(delay), ref delay, ConsoleArg.Parsers.Int);
                carg.TrySet(nameof(startAt), ref startAt, ConsoleArg.Parsers.DateTime);
                carg.TrySet(nameof(logFolder), ref logFolder, ConsoleArg.Parsers.DirectoryPath);
            }
            if (startAt < DateTime.Now) startAt = DateTime.Now.Ceiling(AggregationInterval);
            Console.WriteLine($"Will log active window every {LogInterval:g} starting in {Delay:g}.");
            Console.WriteLine($"Will aggregate activity every {AggregationInterval:g} starting at {StartAt.Time()}.");
            if (Duration is not null) Console.WriteLine($"Will continue for {Duration:g}.");
            Thread.Sleep(Delay);
        }
    }
}

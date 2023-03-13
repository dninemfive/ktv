using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    public static class ConsoleArgs
    {
        public static int Delay = 5;
        public static float LogInterval = 0.25f;
        public static float Duration = -1;
        public static float AggregationInterval = 15;
        public static DateTime StartAt = DateTime.Now.Ceiling(TimeSpan.FromMinutes(AggregationInterval));
        public static void Init(string[] args)
        {
            foreach (string arg in args)
            {
                ConsoleArg carg = new(arg);
                carg.TrySet(nameof(LogInterval), ref LogInterval, ConsoleArg.Parsers.Float);
                carg.TrySet(nameof(Duration), ref Duration, ConsoleArg.Parsers.Float);
                carg.TrySet(nameof(AggregationInterval), ref AggregationInterval, ConsoleArg.Parsers.Float);
                carg.TrySet(nameof(Delay), ref Delay, ConsoleArg.Parsers.Int);
                carg.TrySet(nameof(StartAt), ref StartAt, ConsoleArg.Parsers.DateTime);
            }
            Console.WriteLine($"Beginning ktv. Will log active window title to {Constants.LogPath} every " +
                              $"{LogInterval.Minutes()} for {Duration.Minutes()} starting in {Delay.Seconds()}.");
            Console.WriteLine($"App usage will be aggregated and logged to {"TEMP.ktv.log"} every {AggregationInterval.Minutes()}, starting at {StartAt.Time()}.");
            Thread.Sleep(Delay * 1000);
        }
    }
}

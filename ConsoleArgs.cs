using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    public static class ConsoleArgs
    {        
        public static int Delay => delay;
        private static int delay = 5;
        public static float LogInterval => logInterval;
        private static float logInterval = 0.25f;               
        public static float Duration => duration;
        private static float duration = -1;        
        public static float AggregationInterval => aggregationInterval;
        private static float aggregationInterval = 15;
        public static DateTime StartAt => startAt;
        private static DateTime startAt = DateTime.Now.Ceiling(TimeSpan.FromMinutes(AggregationInterval));        
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
            }
            Console.WriteLine($"Will log active window every {LogInterval.Minutes()} starting in {Delay.Seconds()}.");
            Console.WriteLine($"Will aggregate activity every {AggregationInterval.Minutes()} starting at {StartAt.Time()}.");
            if (Duration >= 0) Console.WriteLine($"Will continue for {Duration.Minutes()}.");
            Thread.Sleep(Delay * 1000);
        }
    }
}

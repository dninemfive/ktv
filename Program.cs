using d9.utl;
using d9.utl.compat;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace d9.ktv;

public class Program
{
    public static class Args
    {
        public static readonly TimeSpan LogInterval
                                = CommandLineArgs.TryGet(nameof(LogInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                               ?? TimeSpan.FromMinutes(0.25);
        public static readonly TimeSpan AggregationInterval
                                = CommandLineArgs.TryGet(nameof(AggregationInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                               ?? TimeSpan.FromMinutes(15);
        public static readonly string CalendarConfigPath = CommandLineArgs.TryGet(nameof(CalendarConfigPath), CommandLineArgs.Parsers.FilePath)
                               ?? "calendar config.json";
    }
    public static DateTime NextLogTime { get; private set; } = DateTime.Now.Ceiling(Args.LogInterval);
    public static DateTime LastAggregationTime { get; private set; } = DateTime.Now;
    public static DateTime NextAggregationTime { get; private set; } = DateTime.Now.Ceiling(Args.AggregationInterval);
    public static int LineNumber { get; private set; } = 0;
    public static DateTime LaunchTime { get; } = DateTime.Now; 
    public static string? LastEventId { get; private set; } = null;
    public static void Main()
    {
        if(1440 % Args.AggregationInterval.TotalMinutes != 0)
        {
            Console.WriteLine($"The aggregation interval must divide the number of minutes in a day evenly, but {Args.AggregationInterval} does not.");
            return;
        }
        Utils.Log($"Logging to `{FileManager.LogFolder.Replace(@"\", "/")}` every {Args.LogInterval.Natural()}; aggregating every {Args.AggregationInterval.Natural()}, " +
                          $"starting at {NextAggregationTime.Time()}.");
        try
        {
            MainLoop();
        }
        finally
        {
            Aggregate(true);
        }
    }
    private static void MainLoop()
    {
        while (true)
        {
            if (DateTime.Now >= NextLogTime)
                RecordActivity();
            if (DateTime.Now >= NextAggregationTime)
                Aggregate();
        }
    }
    private static void RecordActivity()
    {
        ActiveWindowInfo info = ActiveWindow.Info;
        WindowNameLog.Log(info.Program);
        Utils.Log($"{++LineNumber,8}\t{DateTime.Now}\t{info.PrintNull()}");
        NextLogTime = DateTime.Now.Ceiling(Args.LogInterval);
    }
    private static void Aggregate(bool offAggregationTime = false)
    {
        RecordActivity(); // otherwise it doesn't get called before Aggregate :thonk:
        Utils.Log($"{DateTime.Now.Time(),8} (Uptime: {(DateTime.Now - LaunchTime).Natural()}):");
        CalendarManager.LoadConfig();
        foreach ((Activity activity, float proportion) in Activities.Between(LastAggregationTime, offAggregationTime ? DateTime.Now : NextAggregationTime))
            Utils.Log($"\t{$"{proportion:p0}",-8}\t{activity}");
        LastAggregationTime = NextAggregationTime;
        NextAggregationTime += Args.AggregationInterval;
    }    
}
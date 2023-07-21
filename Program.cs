using d9.utl;
using d9.utl.compat;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

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
        public static readonly bool Test = CommandLineArgs.GetFlag(nameof(Test), 't');
    }
    public static DateTime NextLogTime { get; private set; } = DateTime.Now.Ceiling(Args.LogInterval);
    public static DateTime LastAggregationTime { get; private set; } = DateTime.Now;
    public static DateTime NextAggregationTime { get; private set; } = DateTime.Now.Ceiling(Args.AggregationInterval);
    //public static int LineNumber { get; private set; } = 0;
    public static DateTime LaunchTime { get; } = DateTime.Now; 
    public static string? LastEventId { get; private set; } = null;
    public static void Main()
    {
        // https://stackoverflow.com/a/29511342
        // todo: add to utl
        // no longer needed here because the config is accessed when initializing parsers
        // System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(KtvConfig).TypeHandle);
        Parsers.Initialize(KtvConfig.ParserDefs);
        if (Args.Test)
        {
            DateTime delay = DateTime.Now + TimeSpan.FromSeconds(15);
            while (DateTime.Now < delay) ;
            nint activeWindowHandle = ActiveWindow.Handle;
            Console.WriteLine($"Active window handle is {activeWindowHandle}.\n");
            bool? hasFocus(Process process)
            {
                try
                {
                    return process.Handle == activeWindowHandle;
                } 
                catch
                {
                    return null;
                }
            }
            foreach(Process process in Process.GetProcesses().OrderBy(x => x.ProcessName))
            {
                if (hasFocus(process) is not true && string.IsNullOrWhiteSpace(process.MainWindowTitle)) continue;
                Console.Write($"{process.Handle,-8}\t{process.ProcessName,-24}\t{process.MainWindowTitle,-40}");
                Console.WriteLine(hasFocus(process) switch
                {
                    true  => "\t(has focus)",
                    false => "",
                    null  => "\t(handle access denied)"
                });
            }
            return;
        }
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
        // todo: utl: specify that utils.log always writes to console, it simply writes to a file in addition when specified
        Utils.Log($"{DateTime.Now}\t{info.PrintNull()}");
        NextLogTime = DateTime.Now.Ceiling(Args.LogInterval);
    }
    private static void Aggregate(bool offAggregationTime = false)
    {
        RecordActivity(); // otherwise it doesn't get called before Aggregate :thonk:
        Utils.Log($"\n{DateTime.Now.Time()} (Uptime: {(DateTime.Now - LaunchTime).Natural()}):");
        foreach ((Activity activity, float proportion) in Activities.Between(LastAggregationTime, offAggregationTime ? DateTime.Now : NextAggregationTime))
            Utils.Log($"\t{$"{proportion:p0}",-4}\t{activity.WasPosted,-5}\t{activity}");
        LastAggregationTime = NextAggregationTime;
        NextAggregationTime += Args.AggregationInterval;
        Console.WriteLine();
    }    
}
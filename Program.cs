using d9.utl;
using d9.utl.compat;
using System.Text.Json.Serialization;

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
#pragma warning disable CS8618
    public class KtvCalendarConfig
    {
        [JsonInclude]
        public string? Id;
        [JsonInclude]
        public GoogleUtils.EventColor DefaultColor;
        [JsonInclude]
        public HashSet<string> Ignore;
        [JsonInclude]
        public Dictionary<string, GoogleUtils.EventColor> EventColors;
        public override string ToString()
        {
            string result = Id.PrintNull();
            result += $"\n{DefaultColor}";
            result += $"\n{Ignore.ListNotation()}";
            result += $"\n{EventColors.ListNotation()}";
            return result;
        }
    }
#pragma warning restore CS8618
    private static KtvCalendarConfig? CalendarConfig = Config.TryLoad<KtvCalendarConfig>(Args.CalendarConfigPath);
    public static DateTime NextLogTime { get; private set; } = DateTime.Now.Ceiling(Args.LogInterval);
    public static DateTime NextAggregationTime { get; private set; } = DateTime.Now.Ceiling(Args.AggregationInterval);
    public static int LineNumber { get; private set; } = 0;
    public static DateTime LaunchedOn { get; } = DateTime.Today;
    private static List<string>? ExistingAggregateLines = null;
    private static List<ActivityRecord> PreviousRecords = new();
    private static ActivityRecord CurrentRecord = new();
    public static readonly string LogFolder = Path.Join(Config.BaseFolderPath, "logs");
    public static readonly string LogPath = Path.Join(LogFolder, $"{DateTime.Now.Format(TimeFormats.DateTime24H)}.ktv.log");
    public static bool UpdateGoogleCalendar => CalendarConfig is not null && GoogleUtils.HasValidAuthConfig;
    public static string? LastEventId { get; private set; } = null;
    public static void Main()
    {
        Utils.Log($"Logging to `{LogFolder.Replace(@"\", "/")}` every {Args.LogInterval:g}; aggregating every {Args.AggregationInterval:g}, " +
                          $"starting at {NextAggregationTime.Time()}.");
        PerformSetup();
        try
        {
            MainLoop();
        }
        finally
        {
            WriteActivity();
        }
    }
    private static void PerformSetup()
    {
        if (!Directory.Exists(LogFolder))
            Directory.CreateDirectory(LogFolder);
        if (File.Exists(ActivityRecord.AggregateFile(LaunchedOn)))
        {
            string[] lines = File.ReadAllLines(ActivityRecord.AggregateFile(LaunchedOn));
            if (lines.Length > 1)
                ExistingAggregateLines = lines.Skip(1).ToList();
        }
        Utils.DefaultLog = new(LogPath, mode: Log.Mode.WriteImmediate);
    }
    private static IEnumerable<string> DailyActivity(DateTime date)
    {
        yield return ActivityRecord.Header;
        if (ExistingAggregateLines is not null && LaunchedOn == date)
        {
            foreach (string line in ExistingAggregateLines)
                yield return line;
        }
        foreach (ActivityRecord record in PreviousRecords.Where(x => x.Date == date).OrderBy(x => x.StartedAt))
            yield return record.ToString();
    }
    private static void WriteActivity()
    {
        List<DateTime> uniqueDates = PreviousRecords.Select(x => x.Date).ToList();
        foreach (DateTime uniqueDate in uniqueDates)
        {
            string path = ActivityRecord.AggregateFile(uniqueDate);
            File.WriteAllLines(path, DailyActivity(uniqueDate));
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
        CurrentRecord.Log(info.Program);
        Utils.Log($"{++LineNumber,8}\t{DateTime.Now}\t{info.PrintNull()}");
        NextLogTime = DateTime.Now.Ceiling(Args.LogInterval);
    }
    private static void Aggregate()
    {
        Utils.Log($"{DateTime.Now.Time(),8}\t{CurrentRecord.MostCommon}");
        if (!PreviousRecords.Any() || !PreviousRecords.Last().TryMerge(CurrentRecord, CalendarConfig?.Id))
        {
            PreviousRecords.Add(CurrentRecord);
            KtvCalendarConfig? newConfig = Config.TryLoad<KtvCalendarConfig>(Args.CalendarConfigPath);
            if (newConfig is not null && newConfig != CalendarConfig)
            {
                Console.WriteLine($"Loaded new calendar config:\n{newConfig.PrettyPrint()}");
                CalendarConfig = newConfig;
            }
            if (UpdateGoogleCalendar)
            {
                try
                {
                    LastEventId = CurrentRecord.CalendarEvent.SendToCalendar(CalendarConfig!.Id!);
                }
                catch (Exception e)
                {
                    Utils.Log($"Failed to send log to calendar: {e.Message}");
                }
            }
        }
        CurrentRecord = new();
        NextAggregationTime = DateTime.Now.Ceiling(Args.AggregationInterval);
        WriteActivity();
        PreviousRecords = PreviousRecords.Where(x => x.FromToday).ToList();
    }
    public static string ColorIdFor(string activityName)
    {
        GoogleUtils.EventColor color = CalendarConfig!.EventColors.TryGetValue(activityName, out GoogleUtils.EventColor val) ? val : CalendarConfig!.DefaultColor;
        return ((int)color).ToString();
    }
    public static bool Ignore(string activityName) => CalendarConfig?.Ignore.Contains(activityName) ?? false;
}
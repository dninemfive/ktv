// See https://aka.ms/new-console-template for more information
using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Diagnostics;
using System.IO;
namespace d9.ktv
{
    public class Program
    {
        public static class Args
        {
            public static readonly TimeSpan LogInterval
                                    = CommandLineArgs.TryGet(nameof(LogInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                                   ?? TimeSpan.FromMinutes(0.5);
            public static readonly TimeSpan AggregationInterval
                                    = CommandLineArgs.TryGet(nameof(AggregationInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                                   ?? TimeSpan.FromMinutes(15);
            public static readonly string? CalendarId = CommandLineArgs.TryGet(nameof(CalendarId), CommandLineArgs.Parsers.FirstNonNullOrEmptyString);
        }
        public static DateTime NextAggregationTime { get; private set; } = DateTime.Now.Ceiling(Args.AggregationInterval);
        public static int LineNumber { get; private set; } = 0;        
        public static DateTime LaunchedOn { get; } = DateTime.Today;
        private static List<string>? ExistingAggregateLines = null;
        private static List<ActivityRecord> PreviousRecords = new();
        private static ActivityRecord CurrentRecord = new();
        public static readonly string LogFolder = Path.Join(Config.BaseFolderPath, "logs");
        public static readonly string LogPath = Path.Join(LogFolder, $"{DateTime.Now.Format(TimeFormats.DateTime24H)}.ktv.log");
        public static bool UpdateGoogleCalendar => Args.CalendarId is not null && GoogleUtils.ValidConfig;
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
            if (!Directory.Exists(LogFolder)) Directory.CreateDirectory(LogFolder);
            if (File.Exists(ActivityRecord.AggregateFile(LaunchedOn)))
            {
                string[] lines = File.ReadAllLines(ActivityRecord.AggregateFile(LaunchedOn));
                if (lines.Length > 1) ExistingAggregateLines = lines.Skip(1).ToList();
            }
            Utils.DefaultLog = new(LogPath, mode: Log.Mode.WriteImmediate);
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
        private static IEnumerable<string> DailyActivity(DateTime date)
        {
            yield return ActivityRecord.Header;
            if (ExistingAggregateLines is not null && LaunchedOn == date)
            {
                foreach (string line in ExistingAggregateLines) yield return line;
            }
            foreach (ActivityRecord record in PreviousRecords.Where(x => x.Date == date).OrderBy(x => x.StartedAt)) yield return record.ToString();
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
                RecordActivity();
                Utils.Sleep(Args.LogInterval);
                if (DateTime.Now >= NextAggregationTime) Aggregate();
            }
        }
        private static void RecordActivity()
        {
            ActiveWindowInfo info = ActiveWindow.Info;
            CurrentRecord.Log(info.Program);
            Utils.Log($"{++LineNumber,8}\t{DateTime.Now}\t{info.PrintNull()}");
        }
        private static void Aggregate()
        {
            Utils.Log($"{DateTime.Now.Time(),8}\t{CurrentRecord.MostCommon}");
            if (!PreviousRecords.Any() || !PreviousRecords.Last().TryMerge(CurrentRecord))
            {
                PreviousRecords.Add(CurrentRecord);
                if (UpdateGoogleCalendar)
                {
                    Event ev = GoogleUtils.AddEventTo(Args.CalendarId!,                         // known to be non-null because of UpdateGoogleCalendar
                                                      CurrentRecord.MostCommon,
                                                      CurrentRecord.StartedAt.Floor(),
                                                      CurrentRecord.EndedAt!.Value.Ceiling());  // known to be non-null because of check in TryMerge()
                    LastEventId = ev.Id;
                }
            }
            CurrentRecord = new();
            NextAggregationTime += Args.AggregationInterval;
            WriteActivity();
            PreviousRecords = PreviousRecords.Where(x => x.FromToday).ToList();
        }
    }
}
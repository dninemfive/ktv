// See https://aka.ms/new-console-template for more information
using d9.utl;
using System.IO;
namespace d9.ktv
{
    public class Program
    {
        public static TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        public static DateTime NextAggregationTime { get; private set; } = ConsoleArgs.StartAt;
        public static int LineNumber { get; private set; } = 0;
        private static List<string>? ExistingAggregateLines = null;
        public static DateTime LaunchedOn { get; } = DateTime.Today;
        private static List<ActivityRecord> PreviousRecords = new();
        private static ActivityRecord CurrentRecord = new();
        public static void Main()
        {
            ConsoleArgs.Init();
            Utils.DefaultLog = new(ConsoleArgs.LogPath, mode: Log.Mode.WriteImmediate);
            if (File.Exists(ActivityRecord.AggregateFile(LaunchedOn)))
            {
                string[] lines = File.ReadAllLines(ActivityRecord.AggregateFile(LaunchedOn));
                if (lines.Length > 1) ExistingAggregateLines = lines[1..].ToList();
            }
            try
            {
                MainLoop();
            } finally
            {
                WriteActivity();
            }
        }
        private static void Sleep(TimeSpan duration)
        {
            Thread.Sleep((int)duration.TotalMilliseconds);
            Elapsed += duration;
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
            
            while (ConsoleArgs.Duration is null || Elapsed < ConsoleArgs.Duration)
            {
                RecordActivity();
                Sleep(ConsoleArgs.LogInterval);
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
            if (!PreviousRecords.Any() || !PreviousRecords.Last().TryMerge(CurrentRecord)) PreviousRecords.Add(CurrentRecord);
            CurrentRecord = new();
            NextAggregationTime += ConsoleArgs.AggregationInterval;
            WriteActivity();
            PreviousRecords = PreviousRecords.Where(x => x.FromToday).ToList();
        }
    }
}
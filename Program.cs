// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

#region console args

#endregion console args
# region local functions
int ct = 0;
string StringFor(object obj) => $"{++ct,8}\t{DateTime.Now}\t{obj.ToString() ?? obj.PrintNull()}\n";
void Log(string line)
{
    Console.Write(line);
    File.AppendAllText(ConsoleArgs.LogPath, line);
}
static void Sleep(TimeSpan duration, ref TimeSpan elapsed)
{
    Thread.Sleep((int)duration.TotalMilliseconds);
    elapsed += duration;
}
static IEnumerable<string> DailyActivity(IEnumerable<ActivityRecord> records, DateTime date)
{
    yield return ActivityRecord.Header;
    foreach (ActivityRecord record in records.Where(x => x.Date == date).OrderBy(x => x.StartedAt)) yield return record.ToString();
}
static void WriteActivity(IEnumerable<ActivityRecord> records)
{
    //Console.WriteLine($"Writing activity with {records.Count()} records.");
    List<DateTime> uniqueDates = records.Select(x => x.Date).ToList();
    foreach(DateTime uniqueDate in uniqueDates)
    {
        // todo: check if the path already exists and append a (n) instead of overwriting
        // also, put logs in a specified folder (default to subfolder of install path)
        string path = ActivityRecord.AggregateFile(uniqueDate);
        //Console.WriteLine($"\t{path}");
        File.WriteAllLines(path, DailyActivity(records, uniqueDate));
    }
}
#endregion local functions
ConsoleArgs.Init(args);
List<ActivityRecord> previousRecords = new();
ActivityRecord activityRecord = new();
DateTime nextAggregationTime = ConsoleArgs.StartAt;
TimeSpan elapsed = TimeSpan.Zero;
while (ConsoleArgs.Duration is null || elapsed < ConsoleArgs.Duration)
{
    ActiveWindowInfo info = ActiveWindow.Info;
    activityRecord.Log(info.Program);
    Log(StringFor(info));
    Sleep(ConsoleArgs.LogInterval, ref elapsed);
    if(DateTime.Now >= nextAggregationTime)
    {
        string mca = $"{DateTime.Now.Time(),8}\t{activityRecord.MostCommon}";
        Log($"{mca}\n");        
        if (!previousRecords.Any() || !previousRecords.Last().TryMerge(activityRecord)) previousRecords.Add(activityRecord);
        activityRecord = new();
        nextAggregationTime += ConsoleArgs.AggregationInterval;
        WriteActivity(previousRecords);
    }
}
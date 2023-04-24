// See https://aka.ms/new-console-template for more information
using d9.ktv;
using d9.utl;
using System.IO;

#region console args

#endregion console args
# region local functions
int ct = 0;
string StringFor(object obj) => $"{++ct,8}\t{DateTime.Now}\t{obj.ToString() ?? obj.PrintNull()}\n";
List<string>? existingAggregateLines = null;
DateTime launchedOnDate = DateTime.Today;
static void Sleep(TimeSpan duration, ref TimeSpan elapsed)
{
    Thread.Sleep((int)duration.TotalMilliseconds);
    elapsed += duration;
}
IEnumerable<string> DailyActivity(IEnumerable<ActivityRecord> records, DateTime date)
{
    yield return ActivityRecord.Header;
    if(existingAggregateLines is not null && launchedOnDate == date)
    {
        foreach (string line in existingAggregateLines) yield return line;
    }
    foreach (ActivityRecord record in records.Where(x => x.Date == date).OrderBy(x => x.StartedAt)) yield return record.ToString();
}
void WriteActivity(IEnumerable<ActivityRecord> records)
{
    List<DateTime> uniqueDates = records.Select(x => x.Date).ToList();
    foreach(DateTime uniqueDate in uniqueDates)
    {
        string path = ActivityRecord.AggregateFile(uniqueDate);
        File.WriteAllLines(path, DailyActivity(records, uniqueDate));
    }

}
#endregion local functions
ConsoleArgs.Init();
if(File.Exists(ActivityRecord.AggregateFile(launchedOnDate)))
{
    string[] lines = File.ReadAllLines(ActivityRecord.AggregateFile(launchedOnDate));
    if(lines.Length > 1) existingAggregateLines = lines[1..].ToList();
}
List<ActivityRecord> previousRecords = new();
ActivityRecord activityRecord = new();
DateTime nextAggregationTime = ConsoleArgs.StartAt;
TimeSpan elapsed = TimeSpan.Zero;
while (ConsoleArgs.Duration is null || elapsed < ConsoleArgs.Duration)
{
    ActiveWindowInfo info = ActiveWindow.Info;
    activityRecord.Log(info.Program);
    Utils.Log(StringFor(info));
    Sleep(ConsoleArgs.LogInterval, ref elapsed);
    if(DateTime.Now >= nextAggregationTime)
    {
        string mca = $"{DateTime.Now.Time(),8}\t{activityRecord.MostCommon}";
        Utils.Log($"{mca}\n");        
        if (!previousRecords.Any() || !previousRecords.Last().TryMerge(activityRecord)) previousRecords.Add(activityRecord);
        activityRecord = new();
        nextAggregationTime += ConsoleArgs.AggregationInterval;
        WriteActivity(previousRecords);
        previousRecords = previousRecords.Where(x => x.FromToday).ToList();
    }
}
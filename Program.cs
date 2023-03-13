// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

#region console args

#endregion console args
# region local functions
int ct = 0;
string StringFor(object obj) => $"{++ct,8}\t{DateTime.Now}\t{obj.ToString() ?? obj.PrintNull()}\n";
void Log(string line, string? extraPath = null)
{
    Console.Write(line);
    File.AppendAllText(Constants.LogPath, line);
    if(extraPath is not null) File.AppendAllText(extraPath, line);
}
static void Sleep(int milliseconds, ref int elapsed)
{
    Thread.Sleep(milliseconds);
    elapsed += milliseconds;
}
static IEnumerable<string> DailyActivity(IEnumerable<ActivityRecord> records, DateTime date)
{
    yield return ActivityRecord.Header(date);
    foreach (ActivityRecord record in records.Where(x => x.Date == date).OrderBy(x => x.StartedAt)) yield return record.ToString();
}
static void WriteActivity(IEnumerable<ActivityRecord> records)
{
    List<DateTime> uniqueDates = records.Select(x => x.Date).ToList();
    foreach(DateTime uniqueDate in uniqueDates)
    {
        string path = $"{uniqueDate.ToString(TimeFormats.Date)}-aggregate.ktv.log";
        File.WriteAllLines(path, DailyActivity(records, uniqueDate));
    }
}
#endregion local functions
ConsoleArgs.Init(args);
List<ActivityRecord> previousRecords = new();
ActivityRecord activityRecord = new();
DateTime nextAggregationTime = ConsoleArgs.StartAt;
TimeSpan aggregationTimespan = TimeSpan.FromMinutes(ConsoleArgs.AggregationInterval);
int durationMilliseconds = (int)(ConsoleArgs.Duration * Constants.MillisecondsPerMinute),
    millisecondsElapsed = 0;
while (ConsoleArgs.Duration < 0 || millisecondsElapsed < ConsoleArgs.Duration)
{
    ActiveWindowInfo info = ActiveWindow.Info;
    activityRecord.Log(info.Program);
    Log(StringFor(info));
    Sleep(durationMilliseconds, ref millisecondsElapsed);
    if(DateTime.Now > nextAggregationTime)
    {
        string mca = $"{DateTime.Now.Time(),8}\t{activityRecord.MostCommon}";
        Log($"{mca}\n");        
        if (!previousRecords.Any() || previousRecords.Last().TryMerge(activityRecord)) previousRecords.Add(activityRecord);
        activityRecord = new();
        nextAggregationTime += aggregationTimespan;
        WriteActivity(previousRecords);
    }
}
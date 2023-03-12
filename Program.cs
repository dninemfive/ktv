// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

#region console args

#endregion console args
# region local functions
int ct = 0;
string StringFor(object obj) => $"{++ct,8}\t{DateTime.Now}\t{obj.ToString() ?? obj.DefinitelyReadableString()}\n";
void Log(string line, string? extraPath = null)
{
    Console.Write(line);
    File.AppendAllText(Constants.LogPath, line);
    if(extraPath is not null) File.AppendAllText(extraPath, line);
}
static void Sleep(int milliseconds)
{
    Thread.Sleep(milliseconds);
}
#endregion local functions
ConsoleArgs.Init(args);
string logPath = Constants.LogPath;
float logInterval = ConsoleArgs.LogInterval;
float duration = ConsoleArgs.Duration;
int delay = ConsoleArgs.Delay;
float aggregationInterval = ConsoleArgs.AggregationInterval;
string aggregateLogPath = "TEMP.ktv.log";
DateTime startAt = ConsoleArgs.StartAt;
Console.WriteLine($"Beginning ktv. Will log active window title to {logPath} every {logInterval.Minutes()} for {duration.Minutes()} starting in {(delay / 60f).Minutes()}.");
Console.WriteLine($"App usage will be aggregated and logged to {aggregateLogPath} every {aggregationInterval.Minutes()}, starting at {startAt:h:mm tt}.");
Sleep(delay * 1000);
List<string> aggregatedPrograms = new();
DateTime nextAggregationTime = startAt;
TimeSpan aggregationTimespan = TimeSpan.FromMinutes(aggregationInterval);
float elapsed = 0;
while (duration < 0 || elapsed < duration)
{
    ActiveWindowInfo info = ActiveWindow.Info;
    aggregatedPrograms.Add(info.Program);
    Log(StringFor(info));
    Sleep((int)(logInterval * Constants.MillisecondsPerMinute));
    elapsed += logInterval;
    if(DateTime.Now > nextAggregationTime)
    {
        string mca = $"{DateTime.Now:HH:mm}\t{aggregatedPrograms.MostCommon()}";
        Log($"{mca}\n", aggregateLogPath);
        aggregatedPrograms.Clear();
        nextAggregationTime += aggregationTimespan;
    }
}
Console.WriteLine("Done.");
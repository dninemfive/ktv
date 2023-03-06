// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

# region constants
const int MillisecondsPerMinute = 60 * 1000;
string logPath = $"{DateTime.Now:yyyyMMddHHmmss}.ktv.log", aggregateLogPath = $"{DateTime.Now:yyyyMMddHHmmss}-aggregate.ktv.log";
# endregion constants
#region console args
int delay = 5;
float interval = 0.25f;
float duration = -1;
float aggregationInterval = 15;
DateTime startAt = DateTime.Now + TimeSpan.FromMinutes(aggregationInterval);
#endregion console args
# region local functions
int ct = 0;
string StringFor(object obj) => $"{++ct,8}\t{DateTime.Now}\t{obj.ToString() ?? obj.DefinitelyReadableString()}\n";
void Log(string line, string? extraPath = null)
{
    Console.Write(line);
    File.AppendAllText(logPath, line);
    if(extraPath is not null) File.AppendAllText(extraPath, line);
}
static void Sleep(int milliseconds)
{
    Thread.Sleep(milliseconds);
}
#endregion local functions
foreach (string arg in args)
{
    ConsoleArg carg = new(arg);
    carg.TrySet(nameof(interval), ref interval, ConsoleArg.Parsers.Float);
    carg.TrySet(nameof(duration), ref duration, ConsoleArg.Parsers.Float);
    carg.TrySet(nameof(aggregationInterval), ref aggregationInterval, ConsoleArg.Parsers.Float);
    carg.TrySet(nameof(delay), ref delay, ConsoleArg.Parsers.Int);
    carg.TrySet(nameof(startAt), ref startAt, ConsoleArg.Parsers.DateTime);
}
Console.WriteLine($"Beginning ktv. Will log active window title to {logPath} every {interval.Minutes()} for {duration.Minutes()} starting in {(delay / 60f).Minutes()}.");
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
    Sleep((int)(interval * MillisecondsPerMinute));
    elapsed += interval;
    if(DateTime.Now > nextAggregationTime)
    {
        string mca = $"{DateTime.Now:HH:mm}\t{aggregatedPrograms.MostCommon()}";
        Log($"{mca}\n", aggregateLogPath);
        aggregatedPrograms.Clear();
        nextAggregationTime += aggregationTimespan;
    }
}
Console.WriteLine("Done.");
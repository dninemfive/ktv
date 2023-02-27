// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

# region constants
const int MillisecondsPerMinute = 60 * 1000;
string logPath = $"{DateTime.Now:yyyyMMddHHmmss}.ktv.log", aggregateLogPath = $"{DateTime.Now:yyyyMMddHHmmss}-aggregate.ktv.log";
# endregion constants
#region console args
int delay = 5;
float interval = 1;
float duration = interval * 24 * 4;
float aggregationInterval = 15;
#endregion console args
# region local functions
int ct = 0;
void Log(object obj)
{
    string str = obj.ToString() ?? obj.DefinitelyReadableString();
    string line = $"{++ct,8}\t{DateTime.Now}\t{str}\n";
    Console.Write(line);
    File.AppendAllText(logPath, line);
}
static void Sleep(int milliseconds)
{
    Thread.Sleep(milliseconds);
}
#endregion local functions
foreach (string arg in args)
{
    ConsoleArg carg = new(arg);
    if (carg.Try<float?>(nameof(interval), s => float.TryParse(s, out float f) ? f : null) is float f) interval = f;
    if (carg.Try<float?>(nameof(duration), s => float.TryParse(s, out float f) ? f : null) is float g) duration = g;
    if (carg.Try<float?>(nameof(aggregationInterval), s => float.TryParse(s, out float f) ? f : null) is float h) aggregationInterval = h;
    if (carg.Try<int?>(nameof(delay), s => int.TryParse(s, out int i) ? i : null) is int i) delay = i;
}
Console.WriteLine($"Beginning ktv. Will log active window title to {logPath} every {interval.Minutes()} for {duration.Minutes()} starting in {(delay / 60f).Minutes()}.");
Console.WriteLine($"App usage will be aggregated and logged to {aggregateLogPath} every {aggregationInterval.Minutes()}.");
Sleep(delay * 1000);
List<string> aggregatedPrograms = new();
float nextAggregationTime = aggregationInterval;
float elapsed = 0;
while (duration < 0 || elapsed < duration)
{
    ActiveWindowInfo info = ActiveWindow.Info;
    aggregatedPrograms.Add(info.Program);
    Log(info);
    Sleep((int)(interval * MillisecondsPerMinute));
    elapsed += interval;
    if(elapsed >= nextAggregationTime)
    {
        string mca = $"{DateTime.Now:HH:mm}\t{aggregatedPrograms.MostCommon()}";
        File.AppendAllText(aggregateLogPath, $"{mca}\n");
        aggregatedPrograms.Clear();
        nextAggregationTime += aggregationInterval;
    }
}
Console.WriteLine("Done.");
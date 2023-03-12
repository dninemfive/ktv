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
static void Sleep(int milliseconds, ref int elapsed)
{
    Thread.Sleep(milliseconds);
    elapsed += milliseconds;
}
#endregion local functions
ConsoleArgs.Init(args);
Thread.Sleep(ConsoleArgs.Delay * 1000);
List<string> aggregatedPrograms = new();
DateTime nextAggregationTime = ConsoleArgs.StartAt;
TimeSpan aggregationTimespan = TimeSpan.FromMinutes(ConsoleArgs.AggregationInterval);
int durationMilliseconds = (int)(ConsoleArgs.Duration * Constants.MillisecondsPerMinute);
int millisecondsElapsed = 0;
while (ConsoleArgs.Duration < 0 || millisecondsElapsed < ConsoleArgs.Duration)
{
    ActiveWindowInfo info = ActiveWindow.Info;
    aggregatedPrograms.Add(info.Program);
    Log(StringFor(info));
    Sleep(durationMilliseconds, ref millisecondsElapsed);
    if(DateTime.Now > nextAggregationTime)
    {
        string mca = $"{DateTime.Now.Time(),8}\t{aggregatedPrograms.MostCommon()}";
        Log($"{mca}\n", "TEMP.ktv.log");
        aggregatedPrograms.Clear();
        nextAggregationTime += aggregationTimespan;
    }
}
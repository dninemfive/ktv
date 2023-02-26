// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

// ktv [interval] [times]
const int MillisecondsPerMinute = 60 * 1000;
const int Delay = 10;
const bool Debug = false;
float interval = 1;
float duration = interval * 24 * 4;
int aggregationInterval = 15;
string logPath = "ktv.log";

void Log(string str)
{
    string line = $"{DateTime.Now}\t{str}\n";
    Console.Write(line);
    File.AppendAllText(logPath, line);
}
static void PrintSleep(int milliseconds)
{
    if(Debug) Console.Write($"Sleeping for {milliseconds} milliseconds...");
    Thread.Sleep(milliseconds);
    if(Debug) Console.WriteLine("done!");
}
foreach(string arg in args)
{
    ConsoleArg carg = new(arg);
    Console.WriteLine(carg);
    if (carg.Try<float?>(nameof(interval), s => float.TryParse(s, out float f) ? f : null) is float f) interval = f;
    if (carg.Try<float?>(nameof(duration), s => float.TryParse(s, out float f) ? f : null) is float g) duration = g;
    if (carg.Try<int?>(nameof(aggregationInterval), s => int.TryParse(s, out int i) ? i : null) is int i) aggregationInterval = i;
    if (carg.Try(nameof(logPath), s => s) is string s) logPath = s;
}
Console.WriteLine($"\ninterval: {interval}");
Console.WriteLine($"duration: {duration}");
Console.WriteLine($"aggregationInterval: {aggregationInterval}");
Console.WriteLine($"logPath: {logPath}");
return;
Console.WriteLine($"Beginning ktv. Will log active window title at {interval}-minute intervals for {duration} minutes starting in {Delay} seconds.");
float elapsed = 0;
PrintSleep(Delay * 1000);
Console.WriteLine("Logging has begun.");
while(elapsed < duration)
{
    //Log(ActiveWindow.Title.Nullable());
    (string app, string? details)? info = ActiveWindow.Info;
    if(info is not null)
    {
        (string app, string? details) = info.Value;
        if(details is not null)
        {
            Log($"{app}\t{details}");
        }
        else
        {
            Log(app);
        }
    } 
    else
    {
        Log(ActiveWindow.Info.DefinitelyReadableString());
    }
    PrintSleep((int)(interval * MillisecondsPerMinute));
    elapsed += interval;
}
Console.WriteLine("Done.");
﻿// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

// ktv [interval] [times]
const int MillisecondsPerMinute = 60 * 1000;
int delay = 5;
float interval = 1;
float duration = interval * 24 * 4;
int aggregationInterval = 15;
string logPath = $"{DateTime.Now:yyyyMMddHHmmss}.ktv.log";

void Log(string str)
{
    string line = $"{DateTime.Now}\t{str}\n";
    Console.Write(line);
    File.AppendAllText(logPath, line);
}
static void Sleep(int milliseconds)
{
    Thread.Sleep(milliseconds);
}
foreach(string arg in args)
{
    ConsoleArg carg = new(arg);
    if (carg.Try<float?>(nameof(interval), s => float.TryParse(s, out float f) ? f : null) is float f) interval = f;
    if (carg.Try<float?>(nameof(duration), s => float.TryParse(s, out float f) ? f : null) is float g) duration = g;
    if (carg.Try<int?>(nameof(aggregationInterval), s => int.TryParse(s, out int i) ? i : null) is int i) aggregationInterval = i;
    if (carg.Try<int?>(nameof(delay), s => int.TryParse(s, out int i) ? i : null) is int j) delay = j;
    if (carg.Try(nameof(logPath), s => s) is string s) logPath = s;
}
Console.WriteLine($"Beginning ktv. Will log active window title at {interval}-minute intervals for {duration} minutes starting in {delay} seconds.");
float elapsed = 0;
Sleep(delay * 1000);
Console.WriteLine("Logging has begun.");
while(elapsed < duration)
{
    Console.Write($"{elapsed:00.00}\t");
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
    Sleep((int)(interval * MillisecondsPerMinute));
    elapsed += interval;
}
Console.WriteLine("Done.");
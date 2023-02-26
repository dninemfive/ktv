﻿// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

// ktv [interval] [times]
const int MillisecondsPerMinute = 60 * 1000;
int delay = 5;
float interval = 1;
float duration = interval * 24 * 4;
float aggregationInterval = 15;
string logPath = $"{DateTime.Now:yyyyMMddHHmmss}.ktv.log", aggregateLogPath = $"{DateTime.Now:yyyyMMddHHmmss}-aggregate.ktv.log";
int ct = 0;

void Log(string str)
{
    string line = $"{++ct,8}\t{DateTime.Now}\t{str}\n";
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
    if (carg.Try<float?>(nameof(aggregationInterval), s => float.TryParse(s, out float f) ? f : null) is float h) aggregationInterval = h;
    if (carg.Try<int?>(nameof(delay), s => int.TryParse(s, out int i) ? i : null) is int i) delay = i;
    if (carg.Try(nameof(logPath), s => s) is string s) logPath = s;
}
Console.WriteLine($"Beginning ktv. Will log active window title to {logPath} every {interval.Minutes()} for {duration.Minutes()} starting in {(delay / 60f).Minutes()}.");
Console.WriteLine($"App usage will be aggregated and logged to {aggregateLogPath} every {aggregationInterval.Minutes()}.");
float elapsed = 0;
Sleep(delay * 1000);
List<string> mostRecentApps = new(), apps = new();
float nextAggregationTime = aggregationInterval;
while(duration < 0 || elapsed < duration)
{
    (string app, string? details)? info = ActiveWindow.Info;
    if(info is not null)
    {
        (string app, string? details) = info.Value;
        mostRecentApps.Add(app);
        if(details is not null)
        {
            Log($"{app,-30}\t{details}");
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
    if(elapsed >= nextAggregationTime)
    {
        string mca = $"{DateTime.Now:HH:mm}\t{mostRecentApps.MostCommon()}";
        apps.Add(mca);
        File.AppendAllText(aggregateLogPath, $"{mca}\n");
        mostRecentApps.Clear();
        nextAggregationTime += aggregationInterval;
    }
}
if(mostRecentApps.Any())
{
    apps.Add($"{DateTime.Now}\t{mostRecentApps.MostCommon()}");
    mostRecentApps.Clear();
}
foreach (string item in apps) Console.WriteLine(item);
Console.WriteLine("Done.");
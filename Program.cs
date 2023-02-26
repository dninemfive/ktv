// See https://aka.ms/new-console-template for more information
using ktv;
using System.IO;

// ktv [interval] [times]
const int MillisecondsPerMinute = 60 * 1000;
const int Delay = 10;
const string Path = "ktv.log";
const bool Debug = false;
float interval = 1;
float duration = interval * 24 * 4;
int aggregateInterval = 15;

static void Log(string str)
{
    string line = $"{DateTime.Now}\t{str}\n";
    Console.Write(line);
    File.AppendAllText(Path, line);
}
static void PrintSleep(int milliseconds)
{
    if(Debug) Console.Write($"Sleeping for {milliseconds} milliseconds...");
    Thread.Sleep(milliseconds);
    if(Debug) Console.WriteLine("done!");
}

if (args is not null)
{
    if(args.Length > 0)
    {
        if (!float.TryParse(args[0], out interval))
        {
            Console.WriteLine($"Argument 1 was in the wrong format. Expected something which could be parsed as {interval.GetType().Name}, but got {args[0]} instead.");
        }
        if(args.Length > 1 && !float.TryParse(args[1], out duration))
        {
            Console.WriteLine($"Argument 2 was in the wrong format. Expected something which could be parsed as {duration.GetType().Name}, but got {args[1]} instead.");
        }
    }
}
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
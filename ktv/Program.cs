using d9.ktv.ActivityLogger;
using d9.utl;
using d9.utl.compat;
using System.Text.Json;

namespace d9.ktv;

public class Program
{
    public static class Args
    {
        public static readonly TimeSpan LogInterval
                                = CommandLineArgs.TryGet(nameof(LogInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                               ?? TimeSpan.FromMinutes(0.25);
        public static readonly TimeSpan AggregationInterval
                                = CommandLineArgs.TryGet(nameof(AggregationInterval), CommandLineArgs.Parsers.UsingParser(TimeSpan.FromMinutes))
                               ?? TimeSpan.FromMinutes(15);
        public static readonly string ConfigPath = CommandLineArgs.TryGet(nameof(ConfigPath), CommandLineArgs.Parsers.FilePath)
                               ?? "config.json";
        public static readonly bool Test = CommandLineArgs.GetFlag(nameof(Test), 't');
    }
    public static DateTime LaunchTime { get; } = DateTime.Now;
    public static List<ScheduledTask> ScheduledTasks { get; } = new();
    public static List<TaskScheduler> Schedulers { get; private set; } = [];
    public static void Main()
    {
        KtvConfigDef2 exampleConfig = new()
        {
            ActivityTracker = new()
            {
                LogPeriodMinutes = 0.25f,
                AggregationConfig = new()
                {
                    PeriodMinutes = 15,
                    GoogleCalendarId = "<hash>.calendar.google.com",
                    DefaultCategory = new()
                    {
                        Name = "Default",
                        EventColor = GoogleUtils.EventColor.Graphite
                    },
                    CategoryDefs = new()
                    {
                        { 
                            "games", 
                            new()
                            {
                                EventColor = GoogleUtils.EventColor.Banana,
                                ActivityDefs = [
                                    new()
                                    {
                                        Matcher = new()
                                        {
                                            FileNameMatcher = new()
                                            {
                                                ParentFolder = @"C:\Program Files (x86)\Steam\steamapps\common"
                                            }
                                        },
                                        FileNameRegex = @"C:[\\\/]Program Files \(x86\)[\\\/]Steam[\\\/]steamapps[\\\/]common[\\\/](.+)",
                                        Pattern = "{fileName:0}"
                                    }
                                ]
                            } 
                        }
                    }
                }
            },
            ProcessClosers = [
                    new()
                    {
                        PeriodMinutes = 1,
                        TimeConstraint = new()
                        {
                            DaysOfWeek = [
                                DayOfWeek.Monday,
                                DayOfWeek.Tuesday,
                                DayOfWeek.Wednesday,
                                DayOfWeek.Thursday,
                                DayOfWeek.Friday
                            ],
                            StartTime = new(0, 30),
                            EndTime = new(10, 0)
                        },
                        CloseProcesses = new()
                        {
                            FileNameMatcher = new()
                            {
                                ParentFolder = @"C:\Program Files (x86)\Steam\steamapps\common"
                            }
                        },
                        IgnoreProcesses = new()
                        {
                            ProcessNameRegex = @".+[Cc]rash.?[Hh]andler.+"
                        }
                    }
                ]
        };
        File.WriteAllText("config.json", JsonSerializer.Serialize(exampleConfig, Config.DefaultSerializerOptions));
        return;
        DateTime now = DateTime.Now;
        if (Config.TryLoad<KtvConfigDef2>(Args.ConfigPath) is not KtvConfigDef2 config)
        {
            Console.WriteLine($"Could not find valid config at expected path {Path.GetFullPath(Args.ConfigPath)}!");
            return;
        }
        Schedulers = LoadSchedulers(config).ToList();
        Console.WriteLine(Schedulers.MultilineListWithAlignedTitle("schedulers:"));
        foreach(TaskScheduler scheduler in  Schedulers)
            ScheduledTasks.Add(scheduler.NextTask(now));
        try
        {
            while(true)
            {
                // Console.WriteLine(ScheduledTasks.OrderBy(x => x.ScheduledTime).MultilineListWithAlignedTitle("scheduled tasks:"));
                SleepUntilNext(ScheduledTasks);
                now = DateTime.Now;
                foreach (ScheduledTask task in ScheduledTasks.Where(x => x.ScheduledTime < now).ToList())
                {
                    task.Execute();
                    ScheduledTasks.Add(task.Scheduler.NextTask(task.ScheduledTime));
                    ScheduledTasks.Remove(task);
                }
            }
        }
        finally
        {
            // todo: some sort of TryExecuteEarly on ScheduledTask
        }
    }
    public static IEnumerable<TaskScheduler> LoadSchedulers(KtvConfigDef2 config)
    {
        if(config.ActivityTracker is ActivityTrackerConfig atc)
        {
            TimeSpan logPeriod = TimeSpan.FromMinutes(atc.LogPeriodMinutes);
            yield return new ActiveWindowLogger(logPeriod);
            if(atc.AggregationConfig is ActivityAggregationConfig aac)
                yield return new ActiveWindowAggregator(aac);
        }
        if (config.ProcessClosers is List<ProcessCloserConfig> pccs)
        {
            foreach (ProcessCloserConfig pcc in pccs)
                if (pcc.CloseProcesses is not null || pcc.IgnoreProcesses is not null)
                    yield return new ProcessCloser(pcc);
        }
    }
    private static void SleepUntil(DateTime dt)
    {
        Console.WriteLine($"SleepUntil({dt:G})");
        int delay = (int)(dt - DateTime.Now).TotalMilliseconds;
        if(delay > 0)
            Thread.Sleep(delay);
    }
    private static void SleepUntilNext(IEnumerable<ScheduledTask> tasks)
        => SleepUntil(tasks.Select(x => x.ScheduledTime).Min());
}
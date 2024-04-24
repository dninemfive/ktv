using d9.ktv.ActivityLogger;
using d9.utl;
using d9.utl.compat;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        WriteConfig();
        DateTime now = DateTime.Now;
        if (Config.TryLoad<KtvConfig>(Args.ConfigPath) is not KtvConfig config)
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
    public static void WriteConfig()
    {
        KtvConfig config = new()
        {
            ActivityTracker = new()
            {
                LogPeriodMinutes = 0.25f,
                AggregationConfig = new()
                {
                    GoogleCalendar = new()
                    {
                        Id = "<id>",
                        DefaultColor = GoogleUtils.EventColor.Graphite
                    },
                    DefaultCategoryName = "default",
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
                                        Patterns = new()
                                        {
                                            { ProcessPropertyTarget.FileName, @"C:\\Program Files \(x86\)\\Steam\\steamapps\\common\\(.+)\\.+" }
                                        },
                                        Format = "{fileName:0,1}"
                                    },
                                    new()
                                    {
                                        Patterns = new()
                                        {
                                            { ProcessPropertyTarget.MainWindowTitle, @"Minecraft\*? \d+\.\d+" }
                                        },
                                        Format = "{mainWindowTitle:0,1}"
                                    }
                                ]
                            }
                        },
                        {
                            "programming",
                            new()
                            {
                                EventColor = GoogleUtils.EventColor.Grape,
                                ActivityDefs = [
                                    new()
                                    {
                                        Format = "Microsoft Visual Studio"
                                    }
                                ]
                            }
                        },
                        {
                            "social",
                            new()
                            {
                                EventColor = GoogleUtils.EventColor.Blueberry,
                                ActivityDefs = [
                                    new()
                                    {
                                        Format = "Discord"
                                    }
                                ]
                            }
                        },
                        {
                            "media",
                            new()
                            {
                                EventColor = GoogleUtils.EventColor.Tangerine,
                                ActivityDefs = [
                                    new()
                                    {
                                        Patterns = new()
                                        {
                                            { ProcessPropertyTarget.ProcessName, "(foobar2000|firefox)" }
                                        },
                                        Format = "{processName:0,1}"
                                    }
                                ]
                            }
                        },
                        {
                            "productivity",
                            new()
                            {
                                EventColor = GoogleUtils.EventColor.Basil,
                                ActivityDefs = [
                                    new()
                                    {
                                        Patterns = new()
                                        {
                                            { ProcessPropertyTarget.ProcessName, "EXCEL" }
                                        },
                                        Format = "Excel"
                                    },
                                ]
                            }
                        }
                    },
                    PeriodMinutes = 15,
                    Ignore = [
                        new()
                        {
                            Mode = ProcessMatchMode.ProcessNameMatches,
                            Value = "^Idle$"
                        }
                    ]
                }
            },
            ProcessClosers = [
                new()
                {
                    TimeConstraint = new()
                    {
                        DaysOfWeek = "weekdays",
                        StartTime = new(0, 30),
                        EndTime = new(10, 0)
                    },
                    ProcessesToClose = [
                        new() { Mode = ProcessMatchMode.IsInCategory, Value = "games" }
                    ],
                    ProcessesToIgnore = [
                        new() { Mode = ProcessMatchMode.ProcessNameMatches, Value = @".+[Cc]rash.?[Hh]andler.+" }
                    ],
                    PeriodMinutes = 1
                },
                new()
                {
                    TimeConstraint = new()
                    {
                        StartTime = new(0, 30),
                        EndTime = new(7, 0)
                    },
                    ProcessesToClose = [
                        new() { Mode = ProcessMatchMode.IsInCategory, Value = "games" },
                        new() { Mode = ProcessMatchMode.ProcessNameMatches, Value = "foobar2000" },
                        new() { Mode = ProcessMatchMode.FileNameMatches, Value = "Visual Studio" }
                    ],
                    ProcessesToIgnore = [
                        new() { Mode = ProcessMatchMode.ProcessNameMatches, Value = @".+[Cc]rash.?[Hh]andler.+" }
                    ],
                    PeriodMinutes = 1
                },
                new()
                {
                    TimeConstraint = new()
                    {
                        StartTime = new(1, 30),
                        EndTime = new(7, 0)
                    },
                    ProcessesToClose = [
                        new() { Mode = ProcessMatchMode.ProcessNameMatches, Value = "EXCEL" }
                    ],
                    PeriodMinutes = 10
                }
            ]
        };
        File.WriteAllText("config.json", JsonSerializer.Serialize(config, Config.DefaultSerializerOptions));
    }
    public static IEnumerable<TaskScheduler> LoadSchedulers(KtvConfig config)
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
                if (pcc.ProcessesToClose is not null || pcc.ProcessesToIgnore is not null)
                    yield return new ProcessCloser(pcc);
        }
    }
    private static void SleepUntil(DateTime dt)
    {
        // Console.WriteLine($"SleepUntil({dt:G})");
        int delay = (int)(dt - DateTime.Now).TotalMilliseconds;
        if(delay > 0)
            Thread.Sleep(delay);
    }
    private static void SleepUntilNext(IEnumerable<ScheduledTask> tasks)
        => SleepUntil(tasks.Select(x => x.ScheduledTime).Min());
}
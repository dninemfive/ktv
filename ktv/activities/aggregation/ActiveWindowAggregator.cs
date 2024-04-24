using d9.utl;
using System.Text.Json;

namespace d9.ktv.ActivityLogger;
/// <summary>
/// Aggregates the raw data logged by an <see cref="ActiveWindowLogger"/> into a summary of what
/// was happening during the specified time period.
/// </summary>
/// <param name="period">The length of time over which to summarize the raw active window data.</param>
public class ActiveWindowAggregator(ActivityAggregationConfig config) : FixedPeriodTaskScheduler(TimeSpan.FromMinutes(config.PeriodMinutes))
{
    public ActivityAggregationConfig Config { get; private set; } = config;
    public GoogleCalendarEventManager? CalendarEventManager { get; private set; } = GoogleCalendarEventManager.From(config);
    private DateTime _lastAggregationTime = DateTime.Now;
    /// <summary>
    ///     Gets the <see cref="ActiveWindowLogEntry">ActiveWindowLogEntries</see> during the
    ///     specified time period.
    /// </summary>
    /// <param name="start">The start, <b>inclusive</b>, of the time period during which the
    ///     entries were logged.</param>
    /// <param name="end">The end, <b>inclusive</b>, of the time period during which the entries
    ///     were logged.</param>
    /// <returns>The active window log entries during the specified time period, including the
    ///     start and end.</returns>
    /// <remarks>
    ///     This assumes that the files were written normally, i.e. follow the naming scheme in
    ///     <see cref="ActiveWindowLogUtils.FileNameFor(DateTime)"/> and with entries in chronological
    ///     order.
    /// </remarks>
    public static IEnumerable<ActiveWindowLogEntry> EntriesBetween(DateTime start, DateTime end)
    {
        foreach (string fileName in ActiveWindowLogUtils.FileNamesFor(start, end))
        {
            if (!File.Exists(fileName))
                continue;
            foreach (string line in File.ReadAllLines(fileName))
            {
                ActiveWindowLogEntry? entry = JsonSerializer.Deserialize<ActiveWindowLogEntry>(line);
                if (entry is not null && entry.DateTime >= start)
                {
                    if (entry.DateTime <= end)
                        yield return entry;
                    if (entry.DateTime >= end)
                        break;
                }
            }
        }
    }
    protected override ScheduledTask NextTaskInternal(DateTime dateTime)
        => new(dateTime, () => Aggregate(dateTime), this);
    private void Aggregate(DateTime time)
    {
        IEnumerable<ActiveWindowLogEntry> entries = EntriesBetween(time - Period, time);
        Console.WriteLine($"entries: {entries.ListNotation()}");
        if(!entries.Any())
        {
            Console.WriteLine($"Cannot aggregate 0 entries!");
            return;
        }
        DateTime minEntryTime = entries.Select(x => x.DateTime).Min();
        // todo: set up a syntax to parse the active window process name and window name
        CountingDictionary<Activity, int> dict = new();
        foreach (Activity? a in entries.Select(Config.ActivityFor))
            if (a is not null)
                dict.Increment(a);
        Console.WriteLine($"dict:     {dict.Select(x => $"{x.Key}: {x.Value}").ListNotation(brackets: ("{", "}"))}");
        Dictionary<Activity, float> percentages = dict.Select(x => new KeyValuePair<Activity, float>(x.Key, x.Value / (float)dict.Total)).ToDictionary();
        CalendarEventManager?.PostFromSummary(new(percentages, time - Period, time));
        int maxCt = dict.Select(x => x.Value).Max();
        Console.WriteLine($"{DateTime.Now:g} most common activities in the last {(time - entries.Select(x => x.DateTime).Min()).Natural()}:");
        foreach ((Activity key, float percentage) in percentages.OrderByDescending(x => x.Value))
            Console.WriteLine($"\t{percentage,-5:P1}\t{key}");
        _lastAggregationTime = time;
    }
    public override string ToString()
        => $"{nameof(ActiveWindowAggregator)}({Config})";
}

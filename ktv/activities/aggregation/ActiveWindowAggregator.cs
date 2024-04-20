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
        IEnumerable<ActiveWindowLogEntry> entries = EntriesBetween(_lastAggregationTime, time);
        // todo: set up a syntax to parse the active window process name and window name
        CountingDictionary<Activity, int> dict = new();
        foreach (Activity? a in entries.Select(Config.ActivityFor))
            if (a is not null)
                dict.Increment(a);
        int maxCt = dict.Select(x => x.Value).Max();
        Console.WriteLine($"{DateTime.Now:g} most common activities in the last {(time - _lastAggregationTime).Natural()}:");
        foreach ((Activity key, int value) in (IEnumerable<KeyValuePair<Activity, int>>)dict.OrderByDescending(x => x.Value))
            Console.WriteLine($"\t{value / (double)dict.Total,-5:P1}\t{key.Name}");
        _lastAggregationTime = time;
    }
}

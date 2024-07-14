using d9.utl;
using System.Text.Json;

namespace d9.ktv.ActivityLogger;
/// <summary>
/// Aggregates the raw data logged by an <see cref="ActiveWindowLogger"/> into a summary of what
/// was happening during the specified time period.
/// </summary>
/// <param name="period">The length of time over which to summarize the raw active window data.</param>
public class ActiveWindowAggregator(Progress<string> progress, ActivityAggregationConfig config) 
    : FixedPeriodTaskScheduler(progress, TimeSpan.FromMinutes(config.PeriodMinutes))
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
    protected override TaskScheduler NextTaskInternal(DateTime dateTime)
    {
        Aggregate(dateTime);
        return this;
    }
    private void Aggregate(DateTime time)
    {
        IEnumerable<ActiveWindowLogEntry> entries = EntriesBetween(time - Period, time);
        if(entries.Count() < 2)
        {
            Report($"Cannot aggregate fewer than 2 entries!");
            return;
        }
        List<DateTime> timestamps = entries.Select(x => x.DateTime).ToList();
        TimeSpan actualDuration = time - timestamps.Min();
        IReadOnlyDictionary<Activity, float> percentages = PercentagesFrom(CountActivities(entries.Select(Config.ActivityFor)),
                                                                           actualDuration,
                                                                           MinIntervalBetween(timestamps));
        CalendarEventManager?.PostFromSummary(new(percentages, time - Period, time));
        PrintPercentages(percentages, actualDuration);
        _lastAggregationTime = time;
    }
    private static TimeSpan MinIntervalBetween(List<DateTime> timestamps)
    {
        List<TimeSpan> timespans = [];
        for(int i = 1; i < timestamps.Count; i++)
            timespans.Add(timestamps[i] - timestamps[i - 1]);
        return timespans.Min();
    }
    private static IReadOnlyDictionary<Activity, int> CountActivities(IEnumerable<Activity?> activities)
    {
        CountingDictionary<Activity, int> result = new();
        foreach (Activity? a in activities)
            if (a is not null)
                result.Increment(a);
        return result;
    }
    private static IReadOnlyDictionary<Activity, float> PercentagesFrom(IReadOnlyDictionary<Activity, int> counts, TimeSpan duration, TimeSpan minInterval)
    {
        double expectedCount = duration.DivideBy(minInterval);
        Dictionary<Activity, float> result = counts.Select(x => new KeyValuePair<Activity, float>(x.Key, x.Value / (float)expectedCount)).ToDictionary();
        return result;
    }
    private void PrintPercentages(IReadOnlyDictionary<Activity, float> percentages, TimeSpan duration)
    {
        string durationString = duration.RoundToMinutes().Natural();
        if (!percentages.Any())
        {
            Report($"{DateTime.Now:g}\t\t\tno activities in the last {durationString}.");
            return;
        }
        string report = $"{DateTime.Now:g}\t\tmost common activities in the last {duration.Natural()}:\n" +
                        $"{percentages.OrderByDescending(x => x.Value)
                                      .Select(x => $"\t{x.Value,5:P1}\t{x.Key}")
                                      .Aggregate((x, y) => $"{x}\n{y}")}";
        Report(report);
    }
    public override string ToString()
        => $"{nameof(ActiveWindowAggregator)}({Config})";
}

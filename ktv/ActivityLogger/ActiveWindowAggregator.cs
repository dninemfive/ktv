using d9.utl;
using System.Text.Json;

namespace d9.ktv.ActivityLogger;
/// <summary>
/// Aggregates the raw data logged by an <see cref="ActiveWindowLogger"/> into a summary of what
/// was happening during the specified time period.
/// </summary>
/// <param name="period">The length of time over which to summarize the raw active window data.</param>
public class ActiveWindowAggregator(TimeSpan period) : FixedPeriodTaskScheduler(period)
{
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
    private DateTime? _lastAggregationTime = null;
    private void Aggregate(DateTime time)
    {
        _lastAggregationTime ??= time - Period;
        IEnumerable<ActiveWindowLogEntry> entries = EntriesBetween(_lastAggregationTime.Value, time);
        // todo: set up a syntax to parse the active window process name and window name
        CountingDictionary<string, int> dict = new();
        foreach (string? s in entries.Select(x => x?.ProcessName))
            if (s is not null)
                dict.Increment(s);
        int maxCt = dict.Select(x => x.Value).Max();
        Console.WriteLine($"{DateTime.Now:g} most common processes in the last {(time - _lastAggregationTime.Value).Natural()}:");
        foreach ((string key, int value) in (IEnumerable<KeyValuePair<string, int>>)dict.OrderByDescending(x => x.Value))
            Console.WriteLine($"\t{value / (double)dict.Total,-5:P1}\t{key}");
        _lastAggregationTime = time;
    }
}

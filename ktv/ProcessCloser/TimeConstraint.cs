using System.Text.Json.Serialization;

namespace d9.ktv;
public class TimeConstraint
{
    // todo: aliases for "weekdays," "weekends", "[Su][Mo?][Tu][We?][Th][Fr?][Sa]", &c
    [JsonPropertyName("dayOfWeek")]
    public List<DayOfWeek>? DaysOfWeek { get; set; }
    [JsonPropertyName("after")]
    public TimeOnly? StartTime { get; set; }
    [JsonPropertyName("before")]
    public TimeOnly? EndTime { get; set; }
    public bool DayOfWeekMatches(DateTime dt)
        => DaysOfWeek is null || !DaysOfWeek.Any() || DaysOfWeek.Contains(dt.DayOfWeek);
    public bool TimeMatches(DateTime dt)
    // if start time is less than end time, treat normally
    // if start time is greater than end time, look at the other half of the day
    // e.g. 11 pm - 12:30 am 
    {
        TimeOnly currentTime = TimeOnly.FromDateTime(dt),
                 startTime = StartTime ?? TimeOnly.MinValue,
                 endTime = EndTime ?? TimeOnly.MaxValue;
        return startTime < endTime ? currentTime >= startTime && currentTime <= endTime
                                   : currentTime >= endTime || currentTime <= startTime;
    }
    public bool Matches(DateTime dt)
        => DayOfWeekMatches(dt) && TimeMatches(dt);
    public DateTime NextMatchingDateTime(DateTime dt)
    {
        if (Matches(dt))
            return dt;
        DateTime matchingTime = new(DateOnly.FromDateTime(dt), StartTime ?? TimeOnly.MinValue);
        if (matchingTime < dt)
            matchingTime += TimeSpan.FromDays(1);
        if (DaysOfWeek is not null && DaysOfWeek.Count != 0 && !DaysOfWeek.Contains(matchingTime.DayOfWeek))
        {
            int ct = 0;
            while (ct++ < 7 && !DaysOfWeek.Contains(matchingTime.DayOfWeek))
                matchingTime += TimeSpan.FromDays(1);
        }
        return matchingTime;
    }
    public override string ToString()
        => $"{DaysOfWeek?.Abbreviation()} {(StartTime is not null, EndTime is not null) switch
        {
            (true, true) => $"{StartTime:g} - {EndTime:g}",
            (true, false) => $"{StartTime:g}",
            (false, true) => $"{EndTime:g}",
            _ => ""
        }}".Trim();
}
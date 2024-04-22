using System.Text.Json.Serialization;

namespace d9.ktv;
public class TimeConstraint
{
    [JsonPropertyName("on")]
    public string? DaysOfWeek { get; set; }
    [JsonPropertyName("after")]
    public TimeOnly? StartTime { get; set; }
    [JsonPropertyName("before")]
    public TimeOnly? EndTime { get; set; }
    public bool DayOfWeekMatches(DateTime dt)
    {
        if (DaysOfWeek is null)
            return true;
        IEnumerable<DayOfWeek> days = DaysOfWeek.ParseWeekdays();
        return !days.Any() || days.Contains(dt.DayOfWeek);
    }
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
        int ct = 0;
        while (ct++ < 7 && !DayOfWeekMatches(matchingTime))
            matchingTime += TimeSpan.FromDays(1);
        return matchingTime;
    }
    public override string ToString()
        => $"{DaysOfWeek?.ParseWeekdays().Abbreviation()} {(StartTime is not null, EndTime is not null) switch
        {
            (true, true) => $"{StartTime} - {EndTime}",
            (true, false) => $"{StartTime}",
            (false, true) => $"{EndTime}",
            _ => ""
        }}".Trim();
}
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
    public DateTime NextMatchingTime(DateTime dt)
    {
        if (Matches(dt))
            return dt;
        DateTime matchingTime = new(DateOnly.FromDateTime(dt), StartTime ?? TimeOnly.MinValue);
        if (matchingTime < dt)
            matchingTime += TimeSpan.FromDays(1);
        if (DaysOfWeek is null || (DaysOfWeek.Any() && !DaysOfWeek.Contains(matchingTime.DayOfWeek)))
        {
            
        }
    }
}
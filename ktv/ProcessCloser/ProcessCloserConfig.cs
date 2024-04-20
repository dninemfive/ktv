using System.Text.Json.Serialization;

namespace d9.ktv;
public class ProcessCloserConfig
{
    [JsonPropertyName("when")]
    public TimeConstraint? TimeConstraint;
    
}
public class TimeConstraint
{
    public List<DayOfWeek>? DaysOfWeek { get; set; }
    public TimeOnly? StartTime { get; set; }
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
}
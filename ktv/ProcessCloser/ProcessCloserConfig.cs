namespace d9.ktv;
public class ProcessCloserConfig
{
    
}
public abstract class TimeConstraint
{
    public abstract bool Matches(DateTime dt);
}
public class TimeOfDayConstraint : TimeConstraint
{
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public override bool Matches(DateTime dt)
    // if start time is less than end time, treat normally
    // if start time is greater than end time, look at the other half of the day
    // e.g. 11 pm - 12:30 am 
    {
        TimeOnly currentTime = TimeOnly.FromDateTime(dt);
        return StartTime < EndTime ? currentTime >= StartTime && currentTime <= EndTime
                                   : currentTime >= EndTime   || currentTime <= StartTime;
    }
}
public class DayOfWeekConstraint : TimeConstraint
{
    public required List<DayOfWeek> DaysOfWeek { get; set; }
    public override bool Matches(DateTime dt)
        => DaysOfWeek.Contains(dt.DayOfWeek);
}
public class EveryNthDayConstraint : TimeConstraint
{
    public DateTime? RelativeTo { get; set; }
    public required int N { get; set; }
    public override bool Matches(DateTime dt)
    {
        DateTime baseline = RelativeTo ?? Program.LaunchTime;
        int daysSince = (int)(dt - baseline).TotalDays;
        return daysSince % N == 0;
    }
}
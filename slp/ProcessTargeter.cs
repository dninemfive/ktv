using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.slp;
#pragma warning disable CS8618 // properties are initialized by JSON serialization
public class ProcessTargeter
{
    [JsonConstructor]
    public ProcessTargeter()
    { }
    public ProcessTargeter(ProcessTargetType type, string value, string? startLateAt = null, string? endEarlyAt = null)
    {
        TargetType = type;
        Value = value;
        StartLateAt = startLateAt;
        EndEarlyAt = endEarlyAt;
    }
    [JsonInclude]
    public ProcessTargetType TargetType { get; private set; }
    [JsonInclude]
    public string Value { get; private set; }
    [JsonInclude]
    public string? StartLateAt { get; private set; }
    [JsonIgnore]
    private TimeOnly? _lateStartTime = null;
    [JsonIgnore]
    public TimeOnly? LateStartTime
    {
        get
        {
            if (_lateStartTime is null && StartLateAt is not null)
                _lateStartTime = TimeOnly.Parse(StartLateAt);
            return _lateStartTime;
        }
    }
    [JsonInclude]
    public string? EndEarlyAt { get; private set; }
    [JsonIgnore]
    private TimeOnly? _earlyEndTime = null;
    [JsonIgnore]
    public TimeOnly? EarlyEndTime
    {
        get
        {
            if (_earlyEndTime is null && EndEarlyAt is not null)
                _earlyEndTime = TimeOnly.Parse(EndEarlyAt);
            return _earlyEndTime;
        }
    }
    internal static TimeOnly TimeNow => TimeOnly.FromDateTime(DateTime.Now);
    [JsonIgnore]
    public bool AppliesNow => (LateStartTime is null || TimeNow > LateStartTime) && (EarlyEndTime is null || TimeNow < EarlyEndTime);
    public bool Matches(Process t) => AppliesNow && TargetType switch
    {
        ProcessTargetType.MainWindowTitle => t.MainWindowTitleContains(Value),
        ProcessTargetType.ProcessName => t.NameContains(Value),
        ProcessTargetType.ProcessLocation => t.IsInFolder(Value),
        _ => throw new Exception($"{TargetType} ({(int)TargetType}) is not a valid process target type!")
    };
}
using System.Text.Json.Serialization;

namespace d9.ktv;
public class KtvConfig
{
    [JsonPropertyName("logActivities")]
    public ActivityTrackerConfig? ActivityTracker { get; set; }
    [JsonPropertyName("closeProcesses")]
    public List<ProcessCloserConfig>? ProcessClosers { get; set; }
    public static readonly KtvConfig Default = new()
    {
        ActivityTracker = new()
        {
            LogPeriodMinutes = 15
        }
    };
    [JsonIgnore]
    private ProcessMatchModeImplementation? _processMatchModeImplementation = null;
    [JsonIgnore]
    public ProcessMatchModeImplementation ProcessMatchModeImplementation
    {
        get
        {
            _processMatchModeImplementation ??= new(this);
            return _processMatchModeImplementation;
        }
    }
}
using System.Text.Json.Serialization;

namespace d9.ktv;
public class KtvConfigDef2
{
    [JsonPropertyName("logActivities")]
    public ActivityTrackerConfig? ActivityTracker { get; set; }
    [JsonPropertyName("closeProcesses")]
    public List<ProcessCloserConfig>? ProcessClosers { get; set; }
}
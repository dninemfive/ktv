using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityTrackerConfig
{
    public required float LogPeriodMinutes { get; set; }
    [JsonPropertyName("aggregation")]
    public ActivityAggregationConfig? AggregationConfig { get; set; }
}

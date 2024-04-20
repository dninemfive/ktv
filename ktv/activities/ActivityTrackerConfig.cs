using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityTrackerConfig
{
    public required float PeriodMinutes { get; set; }
    [JsonPropertyName("aggregation")]
    public ActivityAggregationConfig? AggregationConfig { get; set; }
}

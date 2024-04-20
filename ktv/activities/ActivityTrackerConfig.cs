using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityTrackerConfig
{
    public required float LogPeriodMinutes { get; set; }
    [JsonPropertyName("aggregate")]
    public ActivityAggregationConfig? AggregationConfig { get; set; }
    public override string ToString()
        => new List<string?>()
        {
            $"log period: {LogPeriodMinutes} minutes",
            AggregationConfig?.ToString()
        }
            .Where(x => x is not null)
            .MultilineListWithAlignedTitle("ActivityTrackerConfig");
}

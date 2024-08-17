using System.Text.Json.Serialization;
namespace d9.ktv;
public class ActivityAggregationConfig
{
    [JsonPropertyName("googleCalendar")]
    public GoogleCalendarConfig? GoogleCalendar { get; set; }
    public List<ProcessMatcherDef>? Ignore { get; set; }
    public required float PeriodMinutes { get; set; }
}
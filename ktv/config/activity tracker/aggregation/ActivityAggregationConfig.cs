using d9.utl;
using d9.utl.compat;
using System.Text.Json.Serialization;
namespace d9.ktv;
public class ActivityAggregationConfig
{
    [JsonPropertyName("googleCalendar")]
    public GoogleCalendarConfig? GoogleCalendar { get; set; }
    public required string DefaultCategoryName { get; set; }
    [JsonPropertyName("categories")]
    public required Dictionary<string, ActivityCategoryDef> CategoryDefs { get; set; }
    public List<ProcessMatcherDef>? Ignore { get; set; }
    public required float PeriodMinutes { get; set; }
    public GoogleUtils.EventColor ColorFor(string category)
    {
        if (CategoryDefs.TryGetValue(category, out ActivityCategoryDef? def) && def.EventColor is GoogleUtils.EventColor color)
            return color;
        return GoogleCalendar?.DefaultColor ?? throw new Exception($"Attempted to get default Google Calendar color without a valid config!");
    }
}

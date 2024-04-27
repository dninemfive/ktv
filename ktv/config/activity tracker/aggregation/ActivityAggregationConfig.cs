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
    public Activity? ActivityFor(ActiveWindowLogEntry awle)
    {
        if (Ignore?.Any(x => x.IsSummaryMatch(awle)) ?? false)
            return null;
        // todo: document that this is how things are ordered since the dictionary is unordered
        foreach ((string categoryName, ActivityCategoryDef category) in CategoryDefs.OrderBy(x => x.Key))
            foreach (ActivityDef activity in category.ActivityDefs)
                if (activity.Name(awle) is string name)
                    return new(name, categoryName);
        string? fallbackName = awle.ProcessName ?? awle.MainWindowTitle ?? awle.ProcessName;
        return fallbackName is not null ? new(fallbackName, DefaultCategoryName) : null;
    }
    public GoogleUtils.EventColor ColorFor(string category)
    {
        if (CategoryDefs.TryGetValue(category, out ActivityCategoryDef? def) && def.EventColor is GoogleUtils.EventColor color)
            return color;
        return GoogleCalendar?.DefaultColor ?? throw new Exception($"Attempted to get default Google Calendar color without a valid config!");
    }
}

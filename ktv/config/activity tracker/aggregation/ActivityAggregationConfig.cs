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
    public List<ProcessMatcher>? Ignore { get; set; }
    public required float PeriodMinutes { get; set; }
    [JsonIgnore]
    public GoogleUtils.EventColor? DefaultColor => GoogleCalendar?.DefaultColor;
    public Activity? ActivityFor(ActiveWindowLogEntry awle)
    {
        if (Ignore?.Any(x => x.Matches(awle)) ?? false)
            return null;
        // todo: document that this is how things are ordered since the dictionary is unordered
        foreach ((string categoryName, ActivityCategoryDef category) in CategoryDefs.OrderBy(x => x.Key))
            foreach (ActivityDef activity in category.ActivityDefs)
                if (activity.Name(awle) is string name)
                    return new(name, categoryName, category.EventColor ?? GoogleCalendar?.GetColorFor(categoryName));
        return new((awle.ProcessName ?? awle.MainWindowTitle ?? awle.FileName).PrintNull(), DefaultCategoryName, GoogleCalendar?.DefaultColor);
    }
}

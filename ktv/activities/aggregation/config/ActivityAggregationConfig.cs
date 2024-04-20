using d9.utl;
using System.Text.Json.Serialization;
namespace d9.ktv;
public class ActivityAggregationConfig
{
    [JsonInclude]
    public string? GoogleCalendarId;
    public required DefaultCategoryDef DefaultCategory { get; set; }
    [JsonPropertyName("categories")]
    public required Dictionary<string, ActivityCategoryDef> CategoryDefs { get; set; }
    public List<ProcessMatcher>? Ignore { get; set; }
    public required float PeriodMinutes { get; set; }
    public Activity? ActivityFor(ActiveWindowLogEntry awle)
    {
        if (Ignore?.Any(x => x.Matches(awle)) ?? false)
            return null;
        // todo: document that this is how things are ordered since the dictionary is unordered
        foreach((string name, ActivityCategoryDef def) in CategoryDefs.OrderBy(x => x.Key))
        {
            if (def.CreateActivityFrom(awle, name) is Activity a)
                return a;
        }
        return new((awle.ProcessName ?? awle.MainWindowTitle ?? awle.FileName).PrintNull(), DefaultCategory.Name, DefaultCategory.EventColor);
    }
    public override string ToString()
    {
        List<string?> items = [
            $"Google calendar? {GoogleCalendarId is not null}",
            DefaultCategory.ToString(),
            [..CategoryDefs.Select(x => $"{x.Key}: {x.Value}")]
        ];
    }
}

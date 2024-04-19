using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;
using System.Diagnostics;
using System.Text.Json.Serialization;
namespace d9.ktv;
public class ActivityConfig
{
    [JsonInclude]
    public string? GoogleCalendarId;
    public required DefaultCategoryDef DefaultCategory { get; set; }
    [JsonPropertyName("categories")]
    public required Dictionary<string, ActivityCategoryDef> CategoryDefs { get; set; }
    public List<ProcessMatcher>? Ignore { get; set; }
    public Activity? ActivityFor(Process? p)
    {
        if (Ignore?.Any(x => x.Matches(p)) ?? false)
            return null;
        // todo: document that this is how things are ordered since the dictionary is unordered
        foreach((string name, ActivityCategoryDef def) in CategoryDefs.OrderBy(x => x.Key))
        {
            if (CreateActivityFrom(p, name, def) is Activity a)
                return a;
        }
        return p is not null ? new(p!.ProcessName, DefaultCategory.Name, DefaultCategory.EventColor)
                             : null;
    }
    private static Activity? CreateActivityFrom(Process? p, string categoryName, ActivityCategoryDef category)
    {
        foreach (ActivityDef activityDef in category.ActivityDefs)
            if (activityDef.Name(p) is string name)
                return new(name, categoryName, category.EventColor);
        return null;
    }
}

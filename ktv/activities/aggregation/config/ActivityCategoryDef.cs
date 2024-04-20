using d9.utl.compat;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityCategoryDef
{
    public GoogleUtils.EventColor? EventColor { get; set; }
    [JsonPropertyName("activities")]
    public required List<ActivityDef> ActivityDefs { get; set; }
    public Activity? CreateActivityFrom(ActiveWindowLogEntry awle, string categoryName)
    {
        foreach (ActivityDef activityDef in ActivityDefs)
            if (activityDef.Name(awle) is string name)
                return new(name, categoryName, EventColor);
        return null;
    }
}
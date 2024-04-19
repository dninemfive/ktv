using d9.utl.compat;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityCategoryDef
{
    [JsonInclude]
    public required string Name { get; set; }
    [JsonInclude]
    public required GoogleUtils.EventColor EventColor { get; set; }
    public required List<ActivityDef> ActivityDefs { get; set; }
    public Activity? CreateActivityFrom(Process? p)
    {
        foreach(ActivityDef activityDef in ActivityDefs)
            if (activityDef.Name(p) is string name)
                return new(name, this);
        return null;
    }
}
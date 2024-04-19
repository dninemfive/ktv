using d9.utl.compat;
using System.Text.Json.Serialization;

namespace d9.ktv.ActivityLogger;
public class ActivityCategoryDef
{
    [JsonInclude]
    public required GoogleUtils.EventColor EventColor { get; set; }
    [JsonInclude]
    public required string Name { get; set; }
}
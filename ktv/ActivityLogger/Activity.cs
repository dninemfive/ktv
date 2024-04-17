using d9.utl.compat;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityCategoryDef
{
    [JsonInclude]
    public required GoogleUtils.EventColor EventColor { get; set; }
    [JsonInclude]
    public required string Name { get; set; }
}
public class Activity
{
    public string? EventId { get; private set; } = null;
    public ActivityCategoryDef Category { get; private set; }
}
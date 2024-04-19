using d9.utl.compat;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityCategoryDef
{
    [JsonInclude]
    public required string Name { get; set; }
    [JsonInclude]
    public required GoogleUtils.EventColor EventColor { get; set; }
}
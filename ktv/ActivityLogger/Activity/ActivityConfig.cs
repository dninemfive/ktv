using d9.utl.compat;
using System.Text.Json.Serialization;
namespace d9.ktv;
public class ActivityConfig
{
    public required ActivityCategoryDef DefaultCategory { get; set; }
    [JsonPropertyName("categories")]
    public required Dictionary<string, GoogleUtils.EventColor> CategoryColors { get; set; }
    public required List<ActivityDef> ActivityDefs { get; set; }
    public ActivityCategoryDef CategoryDefOf(string? key)
        => key is null ? DefaultCategory 
                       : new()
                         {
                            Name = key,
                            EventColor = CategoryColors[key]
                         };
}

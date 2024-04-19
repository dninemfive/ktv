using System.Text.Json.Serialization;
namespace d9.ktv;
public class ActivityConfig
{
    public required ActivityCategoryDef DefaultCategory { get; set; }
    [JsonPropertyName("categories")]
    public required Dictionary<string, ActivityCategoryDef> CategoryDefs { get; set; }
    public ActivityCategoryDef CategoryDefOf(string? key)
    {
        if (key is not null && CategoryDefs.TryGetValue(key, out ActivityCategoryDef? value))
            return value;
        return DefaultCategory;
    }
}

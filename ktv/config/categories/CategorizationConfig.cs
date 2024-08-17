using d9.utl;
using d9.utl.compat.google;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class CategorizationConfig
{
    [JsonPropertyName("default")]
    public required DefaultCategoryConfig DefaultCategory { get; set; }
    [JsonPropertyName("categories")]
    public required Dictionary<string, CategoryDef> CategoryDefs { get; set; }
    public GoogleCalendar.EventColor ColorFor(string category)
    {
        if (CategoryDefs.TryGetValue(category, out CategoryDef? def) && def.EventColor is GoogleCalendar.EventColor color)
            return color;
        return DefaultCategory.Color;
    }
}
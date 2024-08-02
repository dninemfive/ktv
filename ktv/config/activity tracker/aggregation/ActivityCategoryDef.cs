using d9.utl.compat.google;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityCategoryDef
{
    [JsonPropertyName("activities")]
    public required List<ActivityDef> ActivityDefs { get; set; }
    /// <summary>
    /// Retained to simplify JSON configuration at the expense of more complicated code around getting colors.
    /// Should try removing once i put a UI together.
    /// </summary>
    public GoogleCalendar.EventColor? EventColor { get; set; }
    public static implicit operator ActivityCategoryDef(List<ActivityDef> activityDefs)
        => new() { ActivityDefs = activityDefs };
    [JsonIgnore]
    public ProcessMatcher ProcessMatcher
        => (_, value, summary) => ActivityDefs.Any(x => x.IsMatch(summary));
}
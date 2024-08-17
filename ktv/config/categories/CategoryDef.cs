using d9.utl.compat.google;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class CategoryDef
{
    [JsonPropertyName("matches")]
    public required List<ProcessMatchDef> ProcessMatchDefs { get; set; }
    public GoogleCalendar.EventColor? EventColor { get; set; }
    [JsonIgnore]
    public ProcessMatcher ProcessMatcher
        => (_, value, summary) => ProcessMatchDefs.Any(x => x.IsMatch(summary));
}
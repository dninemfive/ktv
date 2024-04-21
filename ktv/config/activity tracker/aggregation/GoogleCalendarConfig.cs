using d9.utl.compat;

namespace d9.ktv;
public class GoogleCalendarConfig
{
    public required string Id { get; set; }
    public required GoogleUtils.EventColor DefaultColor { get; set; }
    public Dictionary<string, GoogleUtils.EventColor>? ActivityColors { get; set; }
    public GoogleUtils.EventColor ColorFor(string categoryName)
        => (ActivityColors?.TryGetValue(categoryName, out GoogleUtils.EventColor color) ?? false) ? color : DefaultColor;
}

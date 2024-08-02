using d9.utl.compat.google;

namespace d9.ktv;
public class GoogleCalendarConfig
{
    public required string Id { get; set; }
    public required GoogleCalendar.EventColor DefaultColor { get; set; }
    public required float ActivityPercentageThreshold { get; set; }
}

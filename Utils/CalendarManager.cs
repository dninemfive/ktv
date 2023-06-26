using d9.utl;
using d9.utl.compat;

namespace d9.ktv;

internal static class CalendarManager
{
    internal static string? PostOrUpdateEvent(string name, DateTime start, DateTime end, string? existingId = null)
    {
        if (Program.CalendarId is null || Program.Ignore(name))
            return null;
        Google.Apis.Calendar.v3.Data.Event ev = new()
        {
            Summary = name,
            Start = start.Round().ToEventDateTime(),
            End = end.Round().ToEventDateTime(),
            ColorId = Program.ColorIdFor(name)
        };
        return ev.SendToCalendar(Program.CalendarId, existingId);
    }
}

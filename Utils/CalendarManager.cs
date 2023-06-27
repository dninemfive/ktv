using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;
using System.Text.Json.Serialization;

namespace d9.ktv;

internal static class CalendarManager
{
    private static KtvCalendarConfig? _config;
    static CalendarManager()
    {
        LoadConfig();
    }
    internal static string? PostOrUpdateEvent(string name, DateTime start, DateTime end, string? existingId = null)
    {
        Console.WriteLine($"PostOrUpdateEvent({name}, {start}, {end}, {existingId.PrintNull()})");
        if (!UseCalendar || Ignore(name))
            return null;
        Event ev = new()
        {
            Summary = name,
            Start = start.Round().ToEventDateTime(),
            End = end.Round().ToEventDateTime(),
            ColorId = ColorIdFor(name)
        };
        return ev.SendToCalendar(CalendarId!, existingId);
    }
    private static string? CalendarId => _config?.Id;
    private static bool UseCalendar => _config is not null && GoogleUtils.HasValidAuthConfig;
    internal static void LoadConfig() => _config = Config.TryLoad<KtvCalendarConfig>(Program.Args.CalendarConfigPath);
    private static string ColorIdFor(string activityName)
    {
        GoogleUtils.EventColor color = _config!.EventColors.TryGetValue(activityName, out GoogleUtils.EventColor val) ? val : _config!.DefaultColor;
        return ((int)color).ToString();
    }
    public static bool Ignore(string activityName) => _config?.Ignore.Contains(activityName) ?? false;
    public static string? SendToCalendar(this Event @event, string calendarId, string? existingEventId = null)
    {
        Console.WriteLine($"SendToCalendar({@event}, {calendarId}, {existingEventId.PrintNull()}");
        if (!UseCalendar || Ignore(@event.Summary))
            return null;
        if (existingEventId is not null)
            return GoogleUtils.UpdateEvent(calendarId, existingEventId, @event).Id;
        return GoogleUtils.AddEventTo(calendarId, @event).Id;
    }
}
// written to by JsonSerializer
#pragma warning disable CS8618
#pragma warning disable CS0649
internal class KtvCalendarConfig
{
    [JsonInclude]
    public string? Id;
    [JsonInclude]
    public GoogleUtils.EventColor DefaultColor;
    [JsonInclude]
    public HashSet<string> Ignore;
    [JsonInclude]
    public Dictionary<string, GoogleUtils.EventColor> EventColors;
    public override string ToString()
    {
        string result = Id.PrintNull();
        result += $"\n{DefaultColor}";
        result += $"\n{Ignore.ListNotation()}";
        result += $"\n{EventColors.ListNotation()}";
        return result;
    }
}
using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace d9.ktv;
internal static class KtvConfig
{
    internal static KtvConfigDef? Config = null;
    static KtvConfig()
    {
        Config = utl.Config.TryLoad<KtvConfigDef>(Program.Args.CalendarConfigPath);
        if(Config is not null)
        {
            // the reverse is so that prepending doesn't change the order
            foreach(WindowNameParser.Def wnpd in Config.WindowNameParsers.Reverse<WindowNameParser.Def>())
            {
                WindowNameParser.List.Insert(0, wnpd);
            }
        }
    }
    #region google stuff
    private static bool UseCalendar => Config is not null && GoogleUtils.HasValidAuthConfig;
    private static string? CalendarId => Config?.Id;
    private static bool Ignore(this string windowTitle) => Config?.Ignore.Contains(windowTitle) ?? false;
    private static string ColorIdFor(string activityName)
    {
        GoogleUtils.EventColor color = Config!.EventColors.TryGetValue(activityName, out GoogleUtils.EventColor val) ? val : Config!.DefaultColor;
        return ((int)color).ToString();
    }
    public static GoogleUtils.EventColor DefaultColor = Config?.DefaultColor ?? default;   
    public static string? SendToCalendar(this Event @event, string calendarId, string? existingEventId = null)
    {
        Console.WriteLine($"SendToCalendar({@event}, {calendarId}, {existingEventId.PrintNull()}");
        if (!UseCalendar || Ignore(@event.Summary))
            return null;
        if (existingEventId is not null)
            return GoogleUtils.UpdateEvent(calendarId, existingEventId, @event).Id;
        return GoogleUtils.AddEventTo(calendarId, @event).Id;
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
    #endregion google stuff
}
// written to by JsonSerializer
#pragma warning disable CS8618
#pragma warning disable CS0649
internal class KtvConfigDef
{
    [JsonInclude]
    public string? Id;
    [JsonInclude]
    public GoogleUtils.EventColor DefaultColor;
    [JsonInclude]
    public HashSet<string> Ignore;
    [JsonInclude]
    public Dictionary<string, GoogleUtils.EventColor> EventColors;
    [JsonInclude]
    public List<WindowNameParser.Def> WindowNameParsers;
}
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
public static class KtvConfig
{
    public static IEnumerable<object> ParserDefs => _config?.parsers ?? [];
    private static readonly KtvConfigDef? _config = null;
    static KtvConfig()
    {
        _config = Config.TryLoad<KtvConfigDef>(Program.Args.CalendarConfigPath);
    }
    #region google stuff
    private static bool UseCalendar => _config is not null && GoogleUtils.HasValidAuthConfig;
    private static string? CalendarId => _config?.Id;
    private static bool Ignore(this string windowTitle) => _config?.Ignore.Contains(windowTitle) ?? false;
    private static string ColorIdFor(string activityName)
    {
        GoogleUtils.EventColor color = _config!.EventColors.TryGetValue(activityName, out GoogleUtils.EventColor val) ? val : _config!.DefaultColor;
        return ((int)color).ToString();
    }
    public static GoogleUtils.EventColor DefaultColor = _config?.DefaultColor ?? default;   
    public static string? SendToCalendar(this Event @event, string calendarId, string? existingEventId = null)
    {
        if (!UseCalendar || Ignore(@event.Summary))
            return null;
        if (existingEventId is not null)
            return GoogleUtils.UpdateEvent(calendarId, existingEventId, @event).Id;
        return GoogleUtils.AddEventTo(calendarId, @event).Id;
    }
    internal static string? PostOrUpdateEvent(string name, DateTime start, DateTime end, string? existingId = null)
    {
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
    public List<object> parsers;
}
internal class KtvConfigDef2
{
    internal class CategoryDef
    {
        [JsonInclude]
        public GoogleUtils.EventColor Color;
        [JsonInclude]
        public List<ActivityDef> Activities;
    }
    [JsonInclude]
    public string? GoogleCalendarId;
    [JsonInclude]
    public GoogleUtils.EventColor DefaultColor;
    [JsonInclude]
    public HashSet<string> Ignore;
    [JsonInclude]
    public Dictionary<string, CategoryDef> Categories;
}
internal class ActivityDef
{
    [JsonInclude]
    string? MatcherDefName;
    [JsonConstructor]
    public ActivityDef(string procName)
    {
        // Matcher ??= new($"Matcher_Generated_{procName}", procName);
    }
}
internal class MatcherDef
{
    [JsonInclude]
    string DefName;
    [JsonInclude]
    MatcherTarget Target;
    [JsonInclude]
    MatcherType Type;
    [JsonInclude]
    string? Value;
    [JsonConstructor]
    internal MatcherDef(string defName, string comparisonValue, MatcherTarget target = MatcherTarget.ProcessName, MatcherType type = MatcherType.StringComparison)
    {
        DefName = defName;
        Value = comparisonValue;
        Target = target;
        Type = type;
    }
}
enum MatcherTarget { WindowTitle, ProcessName, ProcessLocation }
enum MatcherType { Regex, StringComparison }
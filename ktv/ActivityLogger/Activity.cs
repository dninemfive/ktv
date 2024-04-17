using d9.utl.compat;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityCategoryDef
{
    [JsonInclude]
    public required GoogleUtils.EventColor EventColor { get; set; }
    [JsonInclude]
    public required string Name { get; set; }
}
public class Activity(string name, string category, string? eventId = null)
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public string? EventId { get; private set; } = eventId;
}
public class ProcessTransformer
{

}
// [javaw, Minecraft* 1.20.1 - Singleplayer, java path] -> (Games, Minecraft* 1.20.1)
// [any executable name, any executable window title, path in steam folder] -> (executable name, 
using d9.utl.compat;
using System.Diagnostics;
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
// general idea: given a process, give it a normalized name which allows it to be counted accurately, and categorize it so it can have a color on the
// calendar
public class ProcessTransformer
{
    public bool Matches(Process process)
    {
        throw new NotImplementedException();
    }
    public (string name, string category) Apply(Process process) { throw new NotImplementedException(); }
}
// [javaw, Minecraft* 1.20.1 - Singleplayer, java path] -> (Games, Minecraft* 1.20.1)
// [any executable name, any executable window title, path in steam folder] -> (executable name, 

public enum MatchMethod
{
    [ImplementedBy("Exact", typeof(ProcessTransformer), [typeof(string)], typeof(bool))]
    Exact,
    CaseInsensitive,
    Contains,
    Regex
}
public enum NamingMethod
{
    EntireField,
    RegexMatches
}
[AttributeUsage(AttributeTargets.Field)]
public class ImplementedByAttribute(string methodName, Type implementingType, Type[] signature, Type? returnType) : Attribute { }
public abstract class TypedExpression<TIn, TOut>(params TypedExpression<TIn, TOut>[] children)
{
    public abstract TOut? Apply(TIn? value);
}
public class TypedOrExpression<TIn> : TypedExpression<TIn, bool>
{
    public override bool Apply(TIn? value)
    {
        throw new NotImplementedException();
    }
}
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.slp;
#pragma warning disable CS8618 // properties are initialized by JSON serialization
public class ProcessTargeter
{
    [JsonConstructor]
    public ProcessTargeter()
    { }
    public ProcessTargeter(ProcessTargetType type, string value)
    {
        TargetType = type;
        Value = value;
    }
    [JsonInclude]
    public ProcessTargetType TargetType { get; private set; }
    [JsonInclude]
    public string Value { get; private set; }
    public bool Matches(Process t) => TargetType switch
    {
        ProcessTargetType.MainWindowTitle => t.MainWindowTitleContains(Value),
        ProcessTargetType.ProcessName => t.NameContains(Value),
        ProcessTargetType.ProcessLocation => t.IsInFolder(Value),
        _ => throw new Exception($"{TargetType} ({(int)TargetType}) is not a valid process target type!")
    };
}
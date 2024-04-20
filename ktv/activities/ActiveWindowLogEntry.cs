using d9.utl;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.ktv;
[method: JsonConstructor]
public class ActiveWindowLogEntry(DateTime dateTime, string? processName, string? mainWindowTitle, string? fileName)
{
    public ActiveWindowLogEntry(DateTime dateTime, Process? process) : this(dateTime, process?.ProcessName, process?.MainWindowTitle, process?.FileName()) { }
    [JsonInclude]
    public DateTime DateTime { get; private set; } = dateTime;
    [JsonInclude]
    public string? ProcessName { get; private set; } = processName;
    [JsonInclude]
    public string? MainWindowTitle { get; private set; } = mainWindowTitle;
    [JsonInclude]
    public string? FileName { get; private set; } = fileName;
    public override string ToString()
        => new object?[] { ProcessName, MainWindowTitle, FileName }.MultilineListWithAlignedTitle($"{DateTime:g}");
}
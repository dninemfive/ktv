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
        => new object?[] { ProcessName, MainWindowTitle, FileName }.MultilineListWithAlignedTitle($"{DateTime:G}");
    public string? this[ProcessPropertyTarget ppt]
        => ppt switch
        {
            ProcessPropertyTarget.FileName => FileName,
            ProcessPropertyTarget.MainWindowTitle => MainWindowTitle,
            ProcessPropertyTarget.ProcessName => ProcessName,
            _ => throw new ArgumentOutOfRangeException($"{ppt} is not a valid ProcessPropertyTarget value!", nameof(ppt))
        };
    public bool AnyPropertyContains(string s)
    {
        foreach (ProcessPropertyTarget ppt in Enum.GetValues<ProcessPropertyTarget>())
            if (this[ppt] is string property && property.Contains(s))
                return true;
        return false;
    }
}
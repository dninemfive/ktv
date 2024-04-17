using d9.utl;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActiveWindowLogEntry(DateTime dateTime, Process? process)
{
    [JsonInclude]
    public DateTime DateTime { get; private set; } = dateTime;
    [JsonInclude]
    public string? ProcessName { get; private set; } = process?.ProcessName;
    [JsonInclude]
    public string? MainWindowTitle { get; private set; } = process?.MainWindowTitle;
    [JsonInclude]
    public string? FileName { get; private set; } = process?.FileName();
    public override string ToString()
        => $"{DateTime:g}\t{ProcessName.PrintNull()}\t{MainWindowTitle.PrintNull()}\t{FileName.PrintNull()}";
}
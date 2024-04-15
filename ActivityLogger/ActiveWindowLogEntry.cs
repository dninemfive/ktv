using d9.utl;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActiveWindowLogEntry(DateTime dateTime, string? processName, string? mainWindowTitle)
{
    [JsonInclude]
    public DateTime DateTime { get; private set; } = dateTime;
    [JsonInclude]
    public string? ProcessName { get; private set; } = processName;
    [JsonInclude]
    public string? MainWindowTitle { get; private set; } = mainWindowTitle;
    public override string ToString()
        => $"{DateTime:g}\t{ProcessName.PrintNull()}\t{MainWindowTitle.PrintNull()}";
}
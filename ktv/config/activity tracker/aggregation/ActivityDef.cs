using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActivityDef
{
    [JsonPropertyName("match")]
    public required ProcessMatcher Matcher { get; set; }
    public string? FileNameRegex { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    public required string Pattern { get; set; }
    public string? Name(ActiveWindowLogEntry? awle)
    {
        if (!Matcher.Matches(awle))
            return null;
        List<(string name, string? regex, string? value)> sourceVariables = [
            ("fileName", awle.FileName, FileNameRegex),
            ("mainWindowTitle", awle.MainWindowTitle, MainWindowTitleRegex),
            ("processName", awle.ProcessName, ProcessNameRegex)
        ];
        return Pattern.RegexReplace(sourceVariables, "(.+)");
    }
}
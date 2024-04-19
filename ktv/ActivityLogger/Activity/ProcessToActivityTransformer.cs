using System.Diagnostics;
using System.Text.RegularExpressions;

namespace d9.ktv;
public class ProcessToActivityTransformer
{
    public required ProcessMatcher Matcher { get; set; }
    public string? FileNameRegex { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    // e.g. for "Minecraft 1.20*", "Minecraft 1.20* - Singleplayer", "Minecraft 1.20* - Multiplayer"
    // MainWindowTitleRegex: "(Minecraft \d+\\.\d+)\\*?.+"
    // Pattern: "{mainWindowTitle:0}"
    public required string Pattern { get; set; }
    public string? Category { get; set; }
    public string? Name(Process? p)
    {
        if (!Matcher.Matches(p))
            return null;
        List<(string name, string? regex, string? value)> sourceVariables = [
            ("fileName", p.FileName(), FileNameRegex),
            ("mainWindowTitle", p.MainWindowTitle, MainWindowTitleRegex),
            ("processName", p.ProcessName, ProcessNameRegex)
        ];
        return Pattern.RegexReplace(sourceVariables);
    }
    public Activity? CreateActivityFrom(Process? p)
    {
        if (!Matcher.Matches(p))
            return null;
        return new(Name(p)!, Category ?? "Default");
    }
}
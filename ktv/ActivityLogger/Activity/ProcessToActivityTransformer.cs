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
        string result = Pattern;
        List<(string name, string? regex, string? value)> sourceVariables = [
            ("fileName", p.FileName(), FileNameRegex),
            ("mainWindowTitle", p.MainWindowTitle, MainWindowTitleRegex),
            ("processName", p.ProcessName, ProcessNameRegex)
            ];
        foreach ((string name, string? regex, string? value) in sourceVariables)
            result = Replace(result, name, value, regex);
        return result;
    }
    private static string Replace(string pattern, string variableName, string? variableValue, string? regex)
    {
        if (variableValue is null || regex is null)
            return pattern;
        MatchCollection matches = Regex.Matches(variableValue, regex);
        for (int i = 0; i < matches.Count; i++)
            pattern = pattern.Replace($"{{{variableName}:{i}}}", matches[i].Value);
        return pattern;
    }
    public Activity? CreateActivityFrom(Process? p)
    {
        if (!Matcher.Matches(p))
            return null;
        return new(Name(p)!, Category ?? "Default");
    }
}
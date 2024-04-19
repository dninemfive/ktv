using d9.utl;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace d9.ktv;
public class Activity(string name, string category, string? eventId = null)
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public string? EventId { get; private set; } = eventId;
}
public class ProcessMatcher
{
    public class FileName
    {
        [JsonPropertyName("isInFolder")]
        public string? ParentFolder { get; set; }
        [JsonPropertyName("matchesRegex")]
        public string? Regex { get; set; }
        public bool Matches(string? fileName)
            => fileName is null
               || ((ParentFolder is null || fileName.IsInFolder(ParentFolder))
               && fileName.Matches(Regex));
    }
    [JsonPropertyName("fileName")]
    public FileName? FileNameMatcher { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    public bool Matches([NotNullWhen(true)]Process? p)
        => p is not null
           && (FileNameMatcher?.Matches(p.FileName()) ?? true)
           && p.MainWindowTitle.Matches(MainWindowTitleRegex) 
           && p.ProcessName.Matches(ProcessNameRegex);
}
public class ProcessNamer
{
    public required ProcessMatcher Matcher { get; set; }
    public string? FileNameRegex { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    // e.g. for "Minecraft 1.20*", "Minecraft 1.20* - Singleplayer", "Minecraft 1.20* - Multiplayer"
    // MainWindowTitleRegex: "(Minecraft \d+\\.\d+)\\*?.+"
    // Pattern: "{mainWindowTitle:0}"
    public required string Pattern { get; set; }
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
}
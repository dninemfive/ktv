using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace d9.ktv;
public class ActivityDef
{
    public Dictionary<ProcessPropertyTarget, string>? Patterns { get; set; }
    public required string Format { get; set; }
    public IEnumerable<(string name, string? regex, string? value)> MatchesFrom(ActiveWindowLogEntry awle)
    {
        if (Patterns is null)
            yield break;
        foreach ((ProcessPropertyTarget key, string regex) in Patterns)
            yield return (key.ToString().toCamelCase(), awle[key], regex);
    }
    public string? Name(ActiveWindowLogEntry? awle)
    {
        if (awle is null)
            return null;
        if (Patterns is null)
            return awle.AnyPropertyContains(Format) ? Format : null;
        return Format.RegexReplace(MatchesFrom(awle), "(.+)");
    }
}
public enum ProcessPropertyTarget
{
    FileName,
    MainWindowTitle,
    ProcessName
}
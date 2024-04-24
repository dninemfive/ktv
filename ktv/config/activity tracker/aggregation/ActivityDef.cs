using d9.utl;

namespace d9.ktv;
public class ActivityDef
{
    public Dictionary<ProcessPropertyTarget, string>? Patterns { get; set; }
    public required string Format { get; set; }
    public bool IsMatch(ProcessSummary summary)
        => Name(summary) is not null;
    public IEnumerable<(string name, string? regex, string? value)> Matches(ProcessSummary summary)
    {
        if (Patterns is null)
            yield break;
        foreach ((ProcessPropertyTarget key, string regex) in Patterns)
            yield return (key.ToString().toCamelCase(), regex, summary[key]);
    }
    public string? Name(ProcessSummary? summary)
    {
        if (summary is null)
            return null;
        if (Patterns is null)
            return summary.AnyPropertyContains(Format) ? Format : null;
        IEnumerable<(string name, string? regex, string? value)> matches = Matches(summary);
        return matches.Any(x => x.value.IsMatch(x.regex)) ? Format.RegexReplace(Matches(summary), "(.+)") : null;
    }
}
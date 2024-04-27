using MatchTuple = (string name, string? value, string? regex);

namespace d9.ktv;
public class ActivityDef
{
    public Dictionary<ProcessPropertyTarget, string>? Patterns { get; set; }
    public required string Format { get; set; }
    public bool IsMatch(ProcessSummary summary)
        => Name(summary) is not null;
    public IEnumerable<MatchTuple> Matches(ProcessSummary summary)
    {
        if (Patterns is null)
            yield break;
        foreach ((ProcessPropertyTarget key, string regex) in Patterns)
            yield return (key.ToString().toCamelCase(), summary[key], regex);
    }
    public string? Name(ProcessSummary? summary)
    {
        if (summary is null)
            return null;
        if (Patterns is null)
            return summary.AnyPropertyContains(Format) ? Format : null;
        IEnumerable<MatchTuple> matches = Matches(summary);
        return matches.Any(x => x.value.IsMatch(x.regex)) ? Format.RegexReplace(matches, "(.+)") : null;
    }
}
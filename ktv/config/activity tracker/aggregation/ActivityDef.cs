namespace d9.ktv;
public class ActivityDef
{
    public Dictionary<ProcessPropertyTarget, string>? Patterns { get; set; }
    public required string Format { get; set; }
    public bool IsMatch(ProcessSummary summary)
        => Matches(summary).Any();
    public IEnumerable<(string name, string? regex, string? value)> Matches(ProcessSummary summary)
    {
        if (Patterns is null)
            yield break;
        foreach ((ProcessPropertyTarget key, string regex) in Patterns)
            yield return (key.ToString().toCamelCase(), summary[key], regex);
    }
    public string? Name(ActiveWindowLogEntry? awle)
    {
        if (awle is null)
            return null;
        if (Patterns is null)
            return awle.AnyPropertyContains(Format) ? Format : null;
        IEnumerable<(string name, string? regex, string? value)> matches = Matches(awle);
        return matches.Any() ? Format.RegexReplace(Matches(awle), "(.+)") : null;
    }
}
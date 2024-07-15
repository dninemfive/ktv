namespace d9.ktv;
public static class ProcessMatcherExtensions
{
    public static bool IsMatch(this List<ProcessMatcherDef>? matchers, ProcessSummary summary)
        => matchers?.Any(x => x.IsMatch(summary)) ?? false;
    public static bool IsMatch(this List<ProcessMatcherDef>? matchers, ProcessSummary summary, out List<ProcessMatcherDef> matches)
    {
        matches = [];
        if (matchers is null)
            return false;
        foreach (ProcessMatcherDef def in matchers)
        {
            if (def.IsMatch(summary))
                matches.Add(def);
        }
        return matches.Any();
    }
}
using System.Text.RegularExpressions;
using MatchTuple = (string name, string? value, string? regex);

namespace d9.ktv;
public static class RegexExtensions
{
    public static bool IsMatch(this string? s, string? regex)
        => s is not null && regex is not null && Regex.IsMatch(s, regex);
    public static string RegexReplace(this string format, IEnumerable<MatchTuple> variables, string? defaultRegex = null)
    {
        foreach ((string key, string? value, string? regex) in variables)
            format = format.RegexReplace((key, value, regex ?? defaultRegex));
        return format;
    }
    /// <summary>
    /// Replaces every key in the given <paramref name="format"/> consisting of <paramref
    /// name="key"/> with the corresponding match(es) of the given <paramref name="regex"/> on
    /// <paramref name="value"/>, if any.
    /// </summary>
    /// <param name="format">The format string in which keys will be replaced.</param>
    /// <param name="key">
    /// The key to replace. Must be wrapped in curly braces, and may optionally specify a match
    /// index and group index, which are both <b>zero-indexed</b>. <br/><br/> See the description of
    /// <see cref="RegexReplace(string, string, string?, string?)"/> for examples.
    /// </param>
    /// <param name="value">
    /// The string on which the regex will operate in order to produce matches and groups to replace
    /// the specified <paramref name="key"/> with. If this parameter is <see langword="null"/>, the
    /// original <paramref name="format"/> string will be returned without modification.
    /// </param>
    /// <param name="regex">
    /// The regex with which to produce matches and groups from the specified <paramref
    /// name="value"/>. If this parameter is <see langword="null"/>, the original <paramref
    /// name="format"/> string will be returned without modification.
    /// </param>
    /// <remarks>
    /// To specify a key to replace, you must include the key wrapped in curly braces with optional
    /// <b>zero-based</b> match and group indices. <br/><br/>
    /// <para>For example:
    /// <list type="bullet">
    /// <item>
    /// To specify a variable named "example" to be replaced by the first match of a regex, you can
    /// write <c>{example}</c>, <c>{example:0}</c>, or <c>{example:0,0}</c>;
    /// </item>
    /// <item>
    /// To specify a variable named "test" to be replaced by the second group of the second match of
    /// the regex, you can write <c>{test:1,1}</c>.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <returns></returns>
    public static string RegexReplace(this string format, MatchTuple match)
    {
        (string key, string? value, string? regex) = match;
        if (value is null || regex is null)
            return format;
        MatchCollection matches = Regex.Matches(value, regex);
        for (int matchIndex = 0; matchIndex < matches.Count; matchIndex++)
        {
            GroupCollection groups = matches[matchIndex].Groups;
            for (int group = 0; group < groups.Count; group++)
                foreach (string indices in ValidReplacementTargetsIndicesFor(matchIndex, group))
                    format = format.Replace($"{{{key}{indices}}}", groups[group].Value);
        }
        return format;
    }
    private static List<string> ValidReplacementTargetsIndicesFor(int match, int group)
        => (match, group) switch
        {
            ( < 0, _) => throw new IndexOutOfRangeException(nameof(match)),
            (_, < 0) => throw new IndexOutOfRangeException(nameof(group)),
            (0, 0) => ["", ":0", ":0,0"],
            (_, 0) => [$":{match}", $":{match},0"],
            _ => [$":{match},{group}"]
        };
}
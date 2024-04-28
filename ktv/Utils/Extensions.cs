﻿using d9.utl;
using d9.utl.compat;
using System.Text.RegularExpressions;
using MatchTuple = (string name, string? value, string? regex);

namespace d9.ktv;

public enum TitlePosition { First, Last }
public static class Extensions
{
    /// <summary>
    /// Replaces characters which would be invalid in a file name with the specified string,
    /// and trims the result to avoid situations where extra whitespace results in odd behavior,
    /// since the Windows filesystem allows leading and trailing whitespace but e.g. File Explorer
    /// does not.
    /// </summary>
    /// <param name="s">The string whose invalid characters to replace.</param>
    /// <param name="replaceWith">The string with which to replace invalid characters.</param>
    /// <returns>A string which is guaranteed to be safe to use as a file name.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string <paramref name="s"/> 
    /// produces an empty or whitespace string after replacement and trimming.</exception>
    /// <remarks>
    /// Note that this is for <b>file names</b>, and so will replace important character for 
    /// absolute paths, such as forward- and backslashes! In addition, this does not check for the
    /// reserved Windows filenames, which can be found 
    /// <see href="https://stackoverflow.com/a/31976060">here</see>.
    /// </remarks>
    public static string FileNameSafe(this string s, string replaceWith = "")
    {
        string result = s;
        foreach (char c in Path.GetInvalidFileNameChars())
            result = result.Replace($"{c}", replaceWith);
        result = result.Trim();
        if (string.IsNullOrWhiteSpace(result))
            throw new ArgumentException($"File name string `{s}` produced an empty or whitespace string after removing invalid characters for a filename!", nameof(s));
        return result;
    }
    public static bool IsInt(this double d)
        => Math.Abs(d - (int)d) < double.Epsilon;
    public static string MultilineListWithAlignedTitle<T>(this IEnumerable<T?> objects, string title, int tabWidth = 2)
    {
        int padWidth = title.Length + tabWidth + (title.Length % tabWidth);
        string padding = " ".Repeated(padWidth);
        string result = title.PadRight(padWidth);
        return result + objects.Select(x => x.PrintNull()).Aggregate((x, y) => $"{x}\n{padding}{y}");
    }
    public static bool IsMatch(this string? s, string? regex)
        => s is not null && regex is not null && Regex.IsMatch(s, regex);
    public static string RegexReplace(this string format, IEnumerable<MatchTuple> variables, string? defaultRegex = null)
    {
        foreach ((string key, string? value, string? regex) in variables)
            format = format.RegexReplace((key, value, regex ?? defaultRegex));
        return format;
    }
    /// <summary>
    /// Replaces every key in the given <paramref name="format"/> consisting of 
    /// <paramref name="key"/> with the corresponding match(es) of the given 
    /// <paramref name="regex"/> on <paramref name="value"/>, if any.
    /// </summary>
    /// <param name="format">The format string in which keys will be replaced.</param>
    /// <param name="key">
    /// The key to replace. Must be wrapped in curly braces, and may optionally specify a match
    /// index and group index, which are both <b>zero-indexed</b>.<br/><br/>See the description of 
    /// <see cref="RegexReplace(string, string, string?, string?)"/> for examples.
    /// </param>
    /// <param name="value">
    /// The string on which the regex will operate in order to produce matches
    /// and groups to replace the specified <paramref name="key"/> with. If this parameter is
    /// <see langword="null"/>, the original <paramref name="format"/> string will be returned
    /// without modification.
    /// </param>
    /// <param name="regex">The regex with which to produce matches and groups from the specified
    /// <paramref name="value"/>. If this parameter is <see langword="null"/>, the original
    /// <paramref name="format"/> string will be returned without modification.</param>
    /// <remarks>
    /// To specify a key to replace, you must include the key wrapped in curly braces with optional
    /// <b>zero-based</b> match and group indices.<br/><br/>
    /// <para>
    /// For example:
    /// <list type="bullet">
    /// <item>
    /// To specify a variable named "example" to be replaced by
    /// the first match of a regex, you can write <c>{example}</c>, <c>{example:0}</c>, or
    /// <c>{example:0,0}</c>;
    /// </item>
    /// <item>
    /// To specify a variable named "test" to be replaced by the second group of the second match
    /// of the regex, you can write <c>{test:1,1}</c>.
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
            for(int group = 0; group < groups.Count; group++)
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
    public static IEnumerable<T> Cycle<T>(this T initialValue)
        where T : struct, Enum
    {
        IEnumerable<T> allValues = Enum.GetValues<T>();
        bool lessThanInitialValue(T x) => x.CompareTo(initialValue) < 0;
        foreach (T value in allValues.SkipWhile(lessThanInitialValue))
            yield return value;
        foreach (T value in allValues.TakeWhile(lessThanInitialValue))
            yield return value;
    }
    public static string Abbreviation(this DayOfWeek dow)
        => dow.ToString()[0..2];
    public static string Abbreviation(this IEnumerable<DayOfWeek> dows)
        => dows.Distinct().Order().Select(Abbreviation).Aggregate((x, y) => $"{x}{y}");
    public static string ToColorId(this GoogleUtils.EventColor color)
        => ((int)color).ToString();
    public static IEnumerable<DayOfWeek> ParseWeekdays(this string s)
    {
        s = s.ToLower();
        return s switch
        {
            "weekdays" or "weekends" => ParseWeekdayAlias(s),
            _ => ParseWeekdayString(s)
        };
    }
    private static IEnumerable<DayOfWeek> ParseWeekdayAlias(string alias)
        => alias switch
        {
            "weekdays" => [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday],
            "weekends" => [DayOfWeek.Saturday, DayOfWeek.Sunday],
            _ => throw new Exception($"incorrect alias provided. this is a private method, how did you get here?")
        };
    private static IEnumerable<DayOfWeek> ParseWeekdayString(string s)
    {
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            string dayStr = day.ToString().ToLower();
            if (s.Contains(dayStr) || s.Contains(dayStr[0..2]))
                yield return day;
        }
    }
#pragma warning disable IDE1006 // Naming Styles: i think it's funny to have the first character be lowercase here
    public static string toCamelCase(this string s)
#pragma warning restore IDE1006
        => s.Length switch
        {
            0 => s,
            1 => s.ToLower(),
            _ => $"{s[0].ToLower()}{s[1..]}"
        };
}

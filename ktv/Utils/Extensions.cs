using d9.utl;
using System.Text.RegularExpressions;

namespace d9.ktv;

public enum TitlePosition { First, Last }
public static class Extensions
{
    public static (string a, string? b)? SplitOn(this string str, string separator, TitlePosition titlePosition)
    {
        if (str is null)
            return null;
        string[] split = str.Split(separator);
        return (split.Length, titlePosition) switch
        {
            (0, _) => null,
            (1, _) => (str.Trim(), null),
            (_, TitlePosition.First) => (split.First().Trim(), str[(split.First().Length + separator.Length)..].Trim()),
            (_, TitlePosition.Last) => (split.Last().Trim(), str[..^(split.Last().Length + separator.Length)].Trim()),
            _ => throw new ArgumentOutOfRangeException(nameof(titlePosition))
        };
    }
    public static string Time(this DateTime time) => time.ToString(TimeFormats.Time);
    public static string FileNameSafe(this string s, string replaceWith = "")
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            s = s.Replace($"{c}", replaceWith);
        return s;
    }
    public static double DivideBy(this TimeOnly dividend, TimeSpan divisor)
        => dividend.ToTimeSpan() / divisor;
    public static double DivideBy(this DateTime dt, TimeSpan divisor)
        => TimeOnly.FromDateTime(dt).DivideBy(divisor);
    public static bool IsInt(this double d)
        => Math.Abs(d - (int)d) < double.Epsilon;
    private static TimeSpan OneDay = TimeSpan.FromHours(24);
    public static bool DividesDayEvenly(this TimeSpan divisor)
        => (OneDay / divisor).IsInt();
    public static DateTime NextDayAlignedTime(this DateTime dt, TimeSpan ts)
    {
        if (!DividesDayEvenly(ts))
            return dt + ts;
        return dt.Floor(ts) + ts;
    }
    public static string MultilineListWithAlignedTitle<T>(this IEnumerable<T?> objects, string title, int tabWidth = 2)
    {
        int padWidth = title.Length + tabWidth + (title.Length % tabWidth);
        string padding = " ".Repeated(padWidth);
        string result = title.PadRight(padWidth);
        return result + objects.Select(x => x.PrintNull()).Aggregate((x, y) => $"{x}\n{padding}{y}");
    }
    public static bool Matches(this string? s, string? regex)
        => s is not null && regex is not null && Regex.IsMatch(s, regex);
    public static string RegexReplace(this string pattern, IEnumerable<(string variableName, string? variableValue, string? regex)> variables, string? defaultRegex = null)
    {
        foreach ((string name, string? value, string? regex) in variables)
            pattern = pattern.RegexReplace(name, value, regex ?? defaultRegex);
        return pattern;
    }
    /// <summary>
    /// Replaces a key in the given <paramref name="format"/> consisting of 
    /// <paramref name="keyName"/> with the corresponding match of the given 
    /// <paramref name="regex"/> on <paramref name="variableValue"/>, if any.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="keyName"></param>
    /// <param name="variableValue"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    public static string RegexReplace(this string format, string keyName, string? variableValue, string? regex)
    {
        Console.WriteLine($"RegexReplace({format}, {keyName}, {variableValue.PrintNull()}, {regex.PrintNull()})");
        if (variableValue is null || regex is null)
            return format;
        MatchCollection matches = Regex.Matches(variableValue, regex);
        Console.WriteLine($"\tmatches: {matches.ListNotation()}");
        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            Console.WriteLine($"\tMatch {i}:");
            Console.WriteLine($"\t\t{match.Groups.Keys.Zip(match.Groups.Values).Select(x => $"{x.First}: {x.Second}").ListNotation()}");
        }
        Console.WriteLine($"\tresult: {format}");
        return format;
    }
    public static string MatchReplace(this string format, string variableName, MatchCollection matches, int? match = null, int? group = null)
    {
        string value = matches[match ?? 0].Groups[group ?? 0].Value;
        string target = $"{{{variableName}{(match, group) switch
        {
            (null, null) => "",
            (not null, null) => $":{match}",
            (not null, not null) => $":{match},{group}",
            _ => throw new ArgumentException("`match` must be non-null if `group` is non-null!", nameof(match))
        }}}}";
        return format.Replace(target, value);
    }
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
}

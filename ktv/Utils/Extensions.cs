using d9.utl;
using System.Numerics;

namespace d9.ktv;

public enum TitlePosition { First, Last }
internal static class Extensions
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
    public static string MultilineListWithAlignedTitle(this IEnumerable<object?> objects, string title, int tabWidth = 2)
    {
        int padWidth = title.Length + tabWidth + (title.Length % tabWidth);
        string padding = " ".Repeated(padWidth);
        string result = title.PadRight(padWidth);
        return result + objects.Select(x => x.PrintNull()).Aggregate((x, y) => $"{x}\n{padding}{y}");
    }
}

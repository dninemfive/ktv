using d9.utl;
using d9.utl.compat.google;

namespace d9.ktv;

public static class Extensions
{
    public static bool IsInt(this double d)
        => Math.Abs(d - (int)d) < double.Epsilon;
    public static string MultilineListWithAlignedTitle<T>(this IEnumerable<T?> objects, string title, int tabWidth = 2)
    {
        int padWidth = title.Length + tabWidth + (title.Length % tabWidth);
        string padding = " ".Repeated(padWidth);
        string result = title.PadRight(padWidth);
        return result + objects.Select(x => x.PrintNull()).Aggregate((x, y) => $"{x}\n{padding}{y}");
    }
    public static string ToColorId(this GoogleCalendar.EventColor color)
        => ((int)color).ToString();
#pragma warning disable IDE1006 // Naming Styles: i think it's funny to have the first character be lowercase here
    public static string toCamelCase(this string s)
#pragma warning restore IDE1006
        => s.Length switch
        {
            0 => s,
            1 => s.ToLower(),
            _ => $"{s[0].ToLower()}{s[1..]}"
        };
    public static void Report<T>(this Progress<T> progress, T value)
        => ((IProgress<T>)progress).Report(value);
}
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;

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
}

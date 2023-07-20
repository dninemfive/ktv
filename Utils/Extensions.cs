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
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Code from <see href="https://stackoverflow.com/a/1393726">here</see>.</remarks>
    /// <param name="time"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    public static DateTime Ceiling(this DateTime date, TimeSpan? span = null)
    {
        TimeSpan ts = span ?? TimeSpan.FromMinutes(1);
        long ticks = (date.Ticks + ts.Ticks - 1) / ts.Ticks;
        return new(ticks * ts.Ticks, date.Kind);
    }
    public static DateTime Floor(this DateTime date, TimeSpan? span = null)
    {
        TimeSpan ts = span ?? TimeSpan.FromMinutes(1);
        return new((date.Ticks / ts.Ticks) * ts.Ticks, date.Kind);
    }
    public static DateTime Round(this DateTime date, TimeSpan? span = null)
    {
        TimeSpan ts = span ?? TimeSpan.FromMinutes(1);
        if (date.Ticks % ts.Ticks < ts.Ticks / 2)
            return Floor(date, span);
        return Ceiling(date, span);
    }    
    public static string Natural(this TimeSpan ts)
    {
        static string? portion(int amt, string name) => amt switch
        {
            <= 0 => null,
            1    => $"{amt} {name}",
            _    => $"{amt} {name}s"
        };
        List<string?> portions = new()
        {
            portion(ts.Days, "day"),
            portion(ts.Hours, "hour"),
            portion(ts.Minutes, "minute"),
            portion(ts.Seconds, "second")
        };
        return portions.Where(x => x is not null)
                       .Aggregate((x, y) => $"{x}, {y}") 
               ?? "";
    }
}

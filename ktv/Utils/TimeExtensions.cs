using d9.utl;

namespace d9.ktv;
public static class TimeExtensions
{
    public const string DateTimeFormat = "yyyy'-'MM'-'dd' 'HH'-'mm'-'ss";
    private static readonly TimeSpan _oneDay = TimeSpan.FromDays(1);
    public static double DivideBy(this TimeSpan dividend, TimeSpan divisor)
        => dividend.TotalMicroseconds / divisor.TotalMicroseconds;
    public static bool DividesDayEvenly(this TimeSpan divisor)
        => (_oneDay / divisor).IsInt();
    public static string Format(this DateTime dt)
        => dt.ToString(DateTimeFormat);
    public static DateTime NextDayAlignedTime(this DateTime dt, TimeSpan ts)
    {
        if (!DividesDayEvenly(ts))
            return dt + ts;
        return dt.Floor(ts) + ts;
    }
    public static string GenerateLogFile(this DateTime dt)
    {
        string logFolder = "logs".AbsoluteOrInBaseFolder();
        _ = Directory.CreateDirectory(logFolder);
        return Path.Join(logFolder, $"{dt.Format()}.ktv.log");
    }
    public static TimeSpan RoundToMinutes(this TimeSpan ts)
        => TimeSpan.FromMinutes(double.Round(ts.TotalMinutes));
}

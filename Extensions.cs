using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ktv
{
    public enum TitlePosition { First, Last }
    internal static class Extensions
    {
        public static string PrintNull(this object? obj, string text = "(null)") => obj?.ToString() ?? text;
        public static string Readable<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is null) return "(null enumerable)";
            if (!enumerable.Any()) return "[]";
            string result = "";
            foreach (T item in enumerable) result += $"{item}, ";
            return $"[{result[..^2]}]";
        }
        public static (string a, string? b)? SplitOn(this string str, string separator, TitlePosition titlePosition)
        {
            if (str is null) return null;
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
        public static T? Parse<T>(this string str, string key, Func<string, T> parser)
        {
            if(str.SplitOn(ConsoleArg.ArgumentSeparator, TitlePosition.First) is (string, string) notNull)
            {
                (string k, string v) = notNull;
                if (v is null) return default;
                try
                {
                    if (k == key) return parser(v);
                }
                catch
                {
                    return default;
                }
            }
            return default;
        }        
        // 1 minute = 6e10 ns; 1 tick = 1e2 ns; 6e10 / 1e2 = 6e8
        public static long Ticks(this float minutes) => (long)(minutes * 6e8);
        public static string Minutes(this float minutes)
        {
            if (minutes < 0) return "an undefined duration";
            if (minutes == 0) return "0s";
            string result = "";
            int wholeMinutes = (int)minutes;
            if (wholeMinutes > 0) result += $"{wholeMinutes}m";
            float seconds = (minutes - wholeMinutes) * 60;
            int wholeSeconds = (int)seconds;
            if (wholeSeconds > 0) result += $"{(result.Length > 0 ? " " : "")}{wholeSeconds}s";
            int milliseconds = (int)((minutes - wholeMinutes - (wholeSeconds / 60f)) * 1000);
            if (milliseconds > 0) result += $"{(result.Length > 0 ? " " : "")}{milliseconds}ms";
            return result;
        }
        public static string Seconds(this float seconds) => (seconds / 60f).Minutes();
        public static string Seconds(this int seconds) => Seconds(seconds);
        public static string Time(this DateTime time) => time.ToString(TimeFormats.Time);
        public static string Format(this DateTime time, string format) => time.ToString(format);
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Code from <see href="https://stackoverflow.com/a/1393726">here</see>.</remarks>
        /// <param name="time"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static DateTime Round(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new(ticks * span.Ticks, date.Kind);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Code from <see href="https://stackoverflow.com/a/1393726">here</see>.</remarks>
        /// <param name="time"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static DateTime Ceiling(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks, date.Kind);
        }
    }
}

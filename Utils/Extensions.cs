using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using d9.utl.compat;

namespace d9.ktv
{
    public enum TitlePosition { First, Last }
    internal static class Extensions
    {
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
        public static string Time(this DateTime time) => time.ToString(TimeFormats.Time);
        public static string Format(this DateTime time, string format) => time.ToString(format);
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
            if (date.Ticks % ts.Ticks < ts.Ticks / 2) return Floor(date, span);
            return Ceiling(date, span);
        }
        public static string? SendToCalendar(this Event @event, string calendarId, string? existingEventId = null)
        {
            if (!Program.UpdateGoogleCalendar || Program.Ignore(@event.Summary)) return null;
            if (existingEventId is not null) return GoogleUtils.UpdateEvent(calendarId, existingEventId, @event).Id;
            return GoogleUtils.AddEventTo(calendarId, @event).Id;
        }
    }
}

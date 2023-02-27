﻿using System;
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
        public static string DefinitelyReadableString(this object? obj, string text = "(null)") => obj?.ToString() ?? text;
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
        public static string MostCommon(this IEnumerable<string> enumerable)
        {
            Dictionary<string, int> dict = new();
            foreach(string s in enumerable)
            {
                if (!dict.ContainsKey(s)) dict.Add(s, 1);
                else dict[s]++;
            }
            IEnumerable<string> mostCommons = dict.Select(x => (x.Key, x.Value)).Where(x => x.Value == dict.Select(x => x.Value).Max()).Select(x => x.Key);
            if (mostCommons.Count() == 1) return mostCommons.First();
            return mostCommons.OrderBy(x => x).Readable();
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
    }
}

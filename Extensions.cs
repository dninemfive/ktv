﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ktv
{
    internal static class Extensions
    {
        public static string DefinitelyReadableString(this object? obj, string text = "(null)") => obj?.ToString() ?? text;
        public static string Readable<T>(this IEnumerable<T> enumerable)
        {
            string result = "";
            foreach (T item in enumerable) result += $"{item}, ";
            return $"[{result[..^2]}]";
        }
        public static (string a, string? b)? SplitOn(this string str, string separators)
        {
            if (str is null) return null;
            string[] split = str.Split(separators);
            Console.WriteLine(split.Readable());
            return split.Length switch
            {
                0 => null,
                1 => (split[0].Trim(), null),
                _ => (split.Last().Trim(), split[..^1].Aggregate((x, y) => $"{x}{separators[0]}{y}").Trim())
            };
        }
        public static T? Parse<T>(this string str, string key, Func<string, T> parser)
        {
            if(str.SplitOn("=") is (string, string) notNull)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal class ConsoleArg
    {
        public const string ArgumentSeparator = "=";
        public string? Key { get; private set; } = null;
        public string? Value { get; private set; } = null;
        public string? InvalidStr { get; private set; } = null;
        public bool Invalid => InvalidStr is not null;
        public ConsoleArg(string arg)
        {
            (string key, string? value)? tuple = arg.SplitOn(ArgumentSeparator, TitlePosition.First);
            if (tuple is null)
            {
                InvalidStr = arg;
            }
            else
            {
                Key = tuple.Value.key;
                Value = tuple.Value.value;
            }
        }
        public override string ToString() => InvalidStr ?? $"{Key.PrintNull("(null key)")}{ArgumentSeparator}{Value.PrintNull("(null value)")}";
        public T? Try<T>(string key, Func<string, T> parser)
        {
            if (Invalid || Value is null || Key != key)
            {
                return default;
            }
            return parser(Value);
        }
        public delegate PossiblyNull<T> Parser<T>(string key);
        public static class Parsers
        {
            public static Parser<float> Float => key => float.TryParse(key, out float f) ? f : PossiblyNull<float>.Null;
            public static Parser<int> Int => key => int.TryParse(key, out int i) ? i : PossiblyNull<int>.Null;
            public static Parser<DateTime> DateTime => key => System.DateTime.TryParse(key, out DateTime dt) ? dt : ktv.PossiblyNull<System.DateTime>.Null;
        }
        public void TrySet<T>(string key, ref T variable, Parser<T> parser)
        {
            if (Invalid || Value is null || Key != key) return;
            T? val = parser(Value);
            if (val is not null)
            {
                variable = val;
                Console.WriteLine($"{key} = {val}");
            }
        }
    }
}

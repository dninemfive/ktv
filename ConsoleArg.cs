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
            public static Parser<bool> Bool => key => bool.TryParse(key, out bool b) ? b : PossiblyNull<bool>.Null;
            public static Parser<float> Float => key => float.TryParse(key, out float f) ? f : PossiblyNull<float>.Null;
            public static Parser<int> Int => key => int.TryParse(key, out int i) ? i : PossiblyNull<int>.Null;
            public static Parser<DateTime> DateTime => key => System.DateTime.TryParse(key, out DateTime dt) ? dt : ktv.PossiblyNull<System.DateTime>.Null;
            public static Parser<string> DirectoryPath => _directoryPath;
            private static PossiblyNull<string> _directoryPath(string key)
            {
                string path;
                try
                {
                    path = Path.GetFullPath(key);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Log folder was specified as {key}, but it is not a valid path: {e.Message}");
                    return PossiblyNull<string>.Null;
                }
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                // https://stackoverflow.com/a/1395226
                if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    Console.WriteLine($"Log folder was specified as {key}, but it is a file, not a folder.");
                    return PossiblyNull<string>.Null;
                }
                return path;
            }
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

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
        public override string ToString() => InvalidStr ?? $"{Key.DefinitelyReadableString("(null key)")}{ArgumentSeparator}{Value.DefinitelyReadableString("(null value)")}";
        public T? Try<T>(string key, Func<string, T> parser)
        {
            if (Invalid || Value is null || Key != key)
            {
                return default;
            }
            return parser(Value);
        }
    }
}

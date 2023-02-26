﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal class ConsoleArg
    {
        public string? Key { get; private set; } = null;
        public string? Value { get; private set; } = null;
        public string? InvalidStr { get; private set; } = null;
        public bool Invalid => InvalidStr is not null;
        public ConsoleArg(string arg)
        {
            (string key, string? value)? tuple = arg.SplitOn("=", first: true);
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
        public override string ToString() => InvalidStr ?? $"{Key.DefinitelyReadableString("(null key)")}={Value.DefinitelyReadableString("(null value)")}";
        public T? Try<T>(string key, Func<string, T> parser)
        {
            Console.Write($"\"{this}\".Try<{typeof(T).Name}>({key},..): ");
            if (Invalid || Value is null || Key != key)
            {
                Console.WriteLine("failed");
                return default;
            }
            Console.WriteLine("succeeded");
            return parser(Value);
        }
    }
}

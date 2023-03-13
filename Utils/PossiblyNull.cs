using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal class PossiblyNull<T>
    {
        public T? Value { get; set; }
        public static PossiblyNull<T> Null => new() { Value = default }; 
        public static implicit operator T?(PossiblyNull<T> nullable) => nullable.Value;
        public static implicit operator PossiblyNull<T>(T val) => new() { Value = val };
    }
}

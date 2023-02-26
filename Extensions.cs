using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal static class Extensions
    {
        public static string DefinitelyReadableString(this object? obj, string text = "(null)") => obj?.ToString() ?? text;
        public static T? TryParse<T>(this string str, string key)
        {

        }
    }
}

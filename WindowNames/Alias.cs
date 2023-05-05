using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace d9.ktv
{
    internal record Alias
    {
        public static readonly List<Alias> List = new()
        {
            new("Mozilla Firefox.*", "Mozilla Firefox"),
            new(".*Visual Studio", "Visual Studio"),
            new("GNU Image Manipulation Program", "GIMP"),
            new("GIMP Startup", "GIMP"),
            new("Unity.*", "Unity")
        };
        public Regex Matcher { get; private set; }
        public string Replacement { get; private set; }
        public Alias(string matcher, string replacement, RegexOptions options = RegexOptions.Compiled)
        {
            Replacement = replacement;
            Matcher = new(matcher, options);
        }
        public bool Matches(string s) => Matcher.IsMatch(s);
        public static string BestReplacement(string s)
        {
            foreach (Alias alias in List) if (alias.Matches(s)) s = alias.Replacement;
            return s;
        }
    }
}

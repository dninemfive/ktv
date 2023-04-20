using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ktv
{
    internal record WindowNameParser
    {
        public static readonly List<WindowNameParser> List = new()
        {
            new(new("MINGW64:.*", RegexOptions.Compiled), s => s.SplitOn(":", TitlePosition.First)),
            new(new("Minecraft.* - .*", RegexOptions.Compiled), s => s.SplitOn(" - ", TitlePosition.First)),
            new(new("Wargame.* - .*", RegexOptions.Compiled), s => s.SplitOn(" - ", TitlePosition.First)),
            new(new(@".* \[foobar2000]", RegexOptions.Compiled), s => ("foobar2000", s.SplitOn(" [", TitlePosition.Last)?.b)),
            new(new(@".* – GIMP.*", RegexOptions.Compiled), s => s.SplitOn(" – ", TitlePosition.Last)),
            // todo: find a way to automatically figure out first/last?
            new(new(".* — .*", RegexOptions.Compiled), s => s.SplitOn(" — ", TitlePosition.Last)),
            new(new(".* - .*", RegexOptions.Compiled), s => s.SplitOn(" - ", TitlePosition.Last)),
            new(new(".*", RegexOptions.Compiled), s => (s, null))

        };
        public Regex Matcher { get; private set; }
        public Func<string, ActiveWindowInfo> Splitter { get; private set; }
        public WindowNameParser(Regex regex, Func<string, ActiveWindowInfo> splitter)
        {
            Matcher = regex;
            Splitter = splitter;
        }
        public bool Matches(string s) => Matcher.Matches(s).Any();
        public ActiveWindowInfo Split(string s) => Splitter(s);
    }
}

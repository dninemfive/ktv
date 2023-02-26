﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ktv
{
    internal record WindowNameParser
    {
        public static List<WindowNameParser> List = new()
        {
            new(new(".*— Mozilla Firefox.*", RegexOptions.Compiled), s => ("Mozilla Firefox", s.SplitOn("—", TitlePosition.Last)?.b)),
            new(new("MINGW64:.*", RegexOptions.Compiled), s => s.SplitOn(":", TitlePosition.First)),
            new(new(".*- MultiMC 5", RegexOptions.Compiled), s => s.SplitOn("-", TitlePosition.Last)),
            new(new("Minecraft.* - .*", RegexOptions.Compiled), s => s.SplitOn(" - ", TitlePosition.First)),
            new(new(".*", RegexOptions.Compiled), s => (s, null))
        };
        public Regex Matcher { get; private set; }
        public Func<string, (string, string?)?> Splitter { get; private set; }
        public WindowNameParser(Regex regex, Func<string, (string, string?)?> splitter)
        {
            Matcher = regex;
            Splitter = splitter;
        }
        public bool Matches(string s) => Matcher.Matches(s).Any();
        public (string title, string? description)? Split(string s) => Splitter(s);
    }
}

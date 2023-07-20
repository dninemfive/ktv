﻿using d9.utl;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace d9.ktv;

public record Parser
{
    public static IEnumerable<Parser> List => _list;
    private static readonly List<Parser> _list = new();
    public static void Initialize(IEnumerable<Parser> parsers)
    {
        foreach (Parser p in parsers) _list.Add(p);
        _list.Add(new(new(@".* \[foobar2000]", RegexOptions.Compiled), 
                          s => ("foobar2000", s.SplitOn(" [", TitlePosition.Last)?.b)));
        foreach(char c in Constants.Hyphens)
        {
            string delimiter = $".* {c} .*";
            _list.Add(new(new($".*{delimiter}.*", RegexOptions.Compiled), s => s.SplitOn(delimiter, TitlePosition.Last)));
        }
        _list.Add(new(new(".*", RegexOptions.Compiled), s => (s, null)));
    }
    public Regex Matcher { get; private set; }
    public Func<string, ActiveWindowInfo?> Splitter { get; private set; }
    public Parser(Regex regex, Func<string, ActiveWindowInfo?> splitter)
    {
        Matcher = regex;
        Splitter = splitter;
    }
    public bool Matches(string s) => Matcher.IsMatch(s);
    public ActiveWindowInfo? Split(string s) => Splitter(s);
    public bool Try(string s, out ActiveWindowInfo? result)
    {
        result = null;
        if (Matcher.IsMatch(s)) return true;
        result = Split(s);
        return result is not null;
    }
    public record Def
    {
        [JsonInclude]
        public string Title, Delimiter;
        [JsonInclude]
        public TitlePosition TitlePosition;
        [JsonIgnore]
        public readonly Regex Matcher;
        [JsonConstructor]
        public Def(string title, string delimiter, TitlePosition titlePosition)
        {
            Title = title;
            Delimiter = delimiter;
            TitlePosition = titlePosition;
            Matcher = new(TitlePosition switch
            {
                TitlePosition.First => $".*{Title}.*{Delimiter}.*",
                TitlePosition.Last =>  $".*{Delimiter}.*{Title}.*",
                _ => throw new ArgumentOutOfRangeException(nameof(titlePosition))
            }, RegexOptions.Compiled);
        }
        public static implicit operator Parser(Def def) => new(def.Matcher, s => s.SplitOn(def.Delimiter, def.TitlePosition));
    }
}

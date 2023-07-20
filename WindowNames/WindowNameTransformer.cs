using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace d9.ktv;
/*
internal class Rule
{
    private readonly Func<string, bool> _appliesTo;
    private readonly Func<string, string> _transformer;
    public Rule(Func<string, bool> validator, Func<string, string> transformer)
    {
        _appliesTo = validator;
        _transformer = transformer;
    }
    public string? Try(string s) => _appliesTo(s) ? _transformer(s) : null;
    public static readonly Rule Mozilla = new(x => Regex.IsMatch(x, @"- Mozilla"), )
}*/
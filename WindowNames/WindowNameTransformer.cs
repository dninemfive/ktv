using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv.WindowNames;
internal class WindowNameTransformer
{
    internal delegate (string? info, string? remainder) Splitter(string s);
    public IEnumerable<string> Specifics(string s, params Splitter[] splitters)
    {
        foreach(Splitter splitter in splitters)
        {
            (string? info, string? remainder) = splitter(s);
            if (info is not null)
                yield return info;
            if (remainder is not null)
            {
                s = remainder;
            }
            else
            {
                yield break;
            }
        } 
    }
}
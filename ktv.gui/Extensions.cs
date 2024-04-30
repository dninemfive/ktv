using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace d9.ktv.gui;
public static class Extensions
{
    public static Block ToBlock(this object? obj)
    {
        Paragraph result = new()
        {
            Margin = new(0)
        };
        result.Inlines.Add(new Run($"{obj}"));
        return result;
    }
}

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
    public static TimeSpan? Subtract(this TimeSpan? left, TimeSpan? right)
    {
        if(left is TimeSpan left_ && right is TimeSpan right_)
        {
            return left_ - right_;
        }
        return null;
    }
    public static double? DivideBy(this TimeSpan? dividend, TimeSpan? divisor)
    {
        if(dividend is TimeSpan dividend_ && divisor is TimeSpan divisor_)
        {
            return dividend_ / divisor_;
        }
        return null;
    }
    public static string ToStringOrNA(this TimeSpan? ts)
    {
        if (ts is null)
            return "(N/A)";
        return $"{ts:g}";
    }
}

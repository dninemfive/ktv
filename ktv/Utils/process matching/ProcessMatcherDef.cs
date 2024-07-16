using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace d9.ktv;
public class ProcessMatcherDef
{
    public required ProcessMatchMode Mode { get; set; }
    public required string Value { get; set; }
    public override string ToString()
        => $"{Mode} {Value}";
}
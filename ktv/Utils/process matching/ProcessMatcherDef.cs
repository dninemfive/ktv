using d9.utl;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace d9.ktv;
public class ProcessMatcherDef
{
    public KtvConfig? Config { private get; set; }
    public required ProcessMatchMode Mode { get; set; }
    public required string Value { get; set; }
    private bool IsMatchInternal(ProcessSummary summary)
        => Mode.ToDelegate(Value)(Value, summary);
    public bool IsMatch([NotNullWhen(true)] ActiveWindowLogEntry? awle)
        => awle is not null && IsMatchInternal(awle);
    public bool IsMatch([NotNullWhen(true)] Process? p)
        => p is not null && IsMatchInternal(p);
}
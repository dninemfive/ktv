using d9.utl;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace d9.ktv;
public class ProcessMatcherDef
{
    public KtvConfig? Config { private get; set; }
    public required ProcessMatchMode Mode { get; set; }
    public required string Value { get; set; }
    public bool IsMatch(ProcessSummary summary)
        => Mode.ToDelegate()(Value, summary);
    public bool IsSummaryMatch([NotNullWhen(true)] ActiveWindowLogEntry? awle)
        => awle is not null && IsMatch(awle);
    public bool IsSummaryMatch([NotNullWhen(true)] Process? p)
        => p is not null && IsMatch(p);
}
using d9.utl;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class ProcessMatchModeImplementation : EnumImplementation<ProcessMatchMode, ProcessMatcher>
{
    public KtvConfig Config { get; private set; }
    public ProcessMatchModeImplementation(KtvConfig config) : base()
    {
        Config = config;
    }
    private static bool PropertyMatches(string? propertyValue, string regex)
        => propertyValue?.IsMatch(regex) ?? false;
#pragma warning disable CA1822 // Mark members as static: need to be instance members for new EnumImplementation constructor
    public bool IsInFolder(string value, ProcessSummary summary)
        => summary.FileName?.IsInFolder(value) ?? false;
    public bool FileNameMatches(string value, ProcessSummary summary)
        => PropertyMatches(summary.FileName, value);
    public bool MainWindowTitleMatches(string value, ProcessSummary summary)
        => PropertyMatches(summary.MainWindowTitle, value);
    public bool ProcessNameMatches(string value, ProcessSummary summary)
        => PropertyMatches(summary.ProcessName, value);
#pragma warning restore CA1822
    public bool IsInCategory(string value, ProcessSummary summary)
        => Config.ActivityTracker?.AggregationConfig?.CategoryDefs.Any(x => x.Value.ProcessMatcher(this, value, summary)) ?? false;
    public bool IsMatch(ProcessMatcherDef def, ProcessSummary summary)
        => this[def.Mode](this, def.Value, summary);
    public bool AnyMatch(List<ProcessMatcherDef>? matchers, ProcessSummary summary)
        => matchers?.Any(x => IsMatch(x, summary)) ?? false;
    public bool AnyMatch(List<ProcessMatcherDef>? matchers, ProcessSummary summary, out List<ProcessMatcherDef> matches)
    {
        matches = [];
        if (matchers is null)
            return false;
        foreach (ProcessMatcherDef def in matchers)
        {
            if (IsMatch(def, summary))
                matches.Add(def);
        }
        return matches.Any();
    }
}
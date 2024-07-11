using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public static class ProcessMatchUtils
{
    public static KtvConfig? Config { private get; set; }
    public static readonly EnumImplementation<ProcessMatchMode, ProcessMatcher> Implementation = new(typeof(ProcessMatchUtils));
    public static ProcessMatcher ToDelegate(this ProcessMatchMode mode)
        => Implementation[mode];
    public static bool IsInFolder(string value, ProcessSummary summary)
        => summary.FileName?.IsInFolder(value) ?? false;
    private static bool PropertyMatches(string? propertyValue, string regex)
        => propertyValue?.IsMatch(regex) ?? false;
    public static bool FileNameMatches(string value, ProcessSummary summary)
        => PropertyMatches(summary.FileName, value);
    public static bool MainWindowTitleMatches(string value, ProcessSummary summary)
        => PropertyMatches(summary.MainWindowTitle, value);
    public static bool ProcessNameMatches(string value, ProcessSummary summary)
        => PropertyMatches(summary.ProcessName, value);
    public static bool IsInCategory(string value, ProcessSummary summary)
        => Config?.ActivityTracker?.AggregationConfig?.CategoryDefs.Any(x => x.Value.ProcessMatcher(value, summary)) ?? false;
    public static bool IsMatch(this List<ProcessMatcherDef>? matchers, ProcessSummary summary)
        => matchers?.Any(x => x.IsMatch(summary)) ?? false;
}
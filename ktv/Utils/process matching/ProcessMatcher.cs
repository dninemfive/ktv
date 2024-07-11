using d9.utl;

namespace d9.ktv;
public delegate bool ProcessMatcher(string value, ProcessSummary summary);
public static class ProcessMatchUtils
{
    public static KtvConfig? Config { private get; set; }
    public static IReadOnlyDictionary<ProcessMatchMode, ProcessMatcher> Index { get; private set; }
        = new Dictionary<ProcessMatchMode, ProcessMatcher>()
        {
            { ProcessMatchMode.InFolder, IsInFolder },
            { ProcessMatchMode.FileNameMatches, FileNameMatches },
            { ProcessMatchMode.MainWindowTitleMatches, MainWindowTitleMatches },
            { ProcessMatchMode.ProcessNameMatches, ProcessNameMatches }
        };
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
        => false;
    public static ProcessMatcher ToDelegate(this ProcessMatchMode mode, string category)
    {
        if(Index.TryGetValue(mode, out ProcessMatcher? matcher)) return matcher;
        if (Config?.ActivityTracker?.AggregationConfig?.CategoryDefs?.TryGetValue(category, out ActivityCategoryDef? def) ?? false)
            return def.ProcessMatcher;
        throw new ArgumentOutOfRangeException(nameof(mode), $"{mode} is not a valid ProcessMatchMode value!");
    }
    public static bool IsMatch(this List<ProcessMatcherDef>? matchers, ProcessSummary summary)
        => matchers?.Any(x => x.IsMatch(summary)) ?? false;
}
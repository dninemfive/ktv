using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class ProcessMatchModeImplementation : EnumImplementation<ProcessMatchMode, ProcessMatcher>
{
    public KtvConfig Config { get; private set; }
    private static ProcessMatchModeImplementation? _instance = null;
    public static ProcessMatchModeImplementation Instance
    {
        get => _instance ?? throw new Exception($"Attempted to reference ProcessMatchModeImplementation.Instance before it was set!");
        set
        {
            Console.WriteLine($"ALERT: Setting ProcessMatchModeImplementation._instance!");
            _instance = value;
        }
    }
    public ProcessMatchModeImplementation(KtvConfig config) : base()
    {
        Config = config;
        Instance = this;
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
        => Config.ActivityTracker?.AggregationConfig?.CategoryDefs.Any(x => x.Value.ProcessMatcher(value, summary)) ?? false;
}
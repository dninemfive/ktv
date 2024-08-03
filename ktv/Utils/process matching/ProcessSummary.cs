using System.Diagnostics;

namespace d9.ktv;
public class ProcessSummary(string? fileName = null, string? mainWindowTitle = null, string? processName = null)
{
    public string? FileName { get; private set; } = fileName;
    public string? MainWindowTitle { get; private set; } = mainWindowTitle;
    public string? ProcessName { get; private set; } = processName;
    public ProcessSummary(Process p) : this(p.FileName(), p.MainWindowTitle, p.ProcessName) { }
    public ProcessSummary(ActiveWindowLogEntry awle) : this(awle.FileName, awle.MainWindowTitle, awle.ProcessName) { }
    public void Deconstruct(out string? fileName, out string? mainWindowTitle, out string? processName)
    {
        fileName = FileName;
        mainWindowTitle = MainWindowTitle;
        processName = ProcessName;
    }
    public string? this[ProcessPropertyTarget ppt]
        => ppt switch
        {
            ProcessPropertyTarget.FileName => FileName,
            ProcessPropertyTarget.MainWindowTitle => MainWindowTitle,
            ProcessPropertyTarget.ProcessName => ProcessName,
            _ => throw new ArgumentOutOfRangeException(nameof(ppt), $"{ppt} is not a valid ProcessPropertyTarget value!")
        };
    public static implicit operator ProcessSummary(ActiveWindowLogEntry? awle)
        => awle is not null ? new(awle) : new();
    public static implicit operator ProcessSummary(Process? process)
        => process is not null ? new(process) : new();
    public bool AnyPropertyContains(string s)
    {
        foreach (ProcessPropertyTarget ppt in Enum.GetValues<ProcessPropertyTarget>())
            if (this[ppt] is string property && property.Contains(s))
                return true;
        return false;
    }
    public override string ToString()
        => $"`{FileName}`, `{MainWindowTitle}`, `{ProcessName}`";
}

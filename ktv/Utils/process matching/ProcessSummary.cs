using System.Diagnostics;

namespace d9.ktv;
public class ProcessSummary(string? fileName, string? mainWindowTitle, string? processName)
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
    public static implicit operator ProcessSummary(ActiveWindowLogEntry awle)
        => new(awle);
    public static implicit operator ProcessSummary(Process process)
        => new(process);
}

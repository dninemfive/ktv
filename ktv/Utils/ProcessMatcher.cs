using d9.utl;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ProcessMatcher
{
    public required ProcessMatcherMode Mode { get; set; }
    public required string Value { get; set; }
    public bool Matches(string? fileName, string? mainWindowTitle, string? processName)
        => Mode switch 
        {
            ProcessMatcherMode.InFolder => fileName?.IsInFolder(Value) ?? false,
            ProcessMatcherMode.FileNameMatches => fileName?.Matches(Value) ?? false,
            ProcessMatcherMode.MainWindowTitleMatches => mainWindowTitle?.Matches(Value) ?? false,
            ProcessMatcherMode.ProcessNameMatches => processName?.Matches(Value) ?? false,
            _ => throw new Exception($"{Mode} is not a valid value for enum ProcessMatcherMode!")
        };
    public bool Matches([NotNullWhen(true)] Process? p)
        => p is not null && Matches(p.FileName(), p.MainWindowTitle, p.ProcessName);
    public bool Matches([NotNullWhen(true)] ActiveWindowLogEntry? awle)
        => awle is not null && Matches(awle.FileName, awle.MainWindowTitle, awle.ProcessName);
}
public enum ProcessMatcherMode
{
    InFolder,
    FileNameMatches,
    MainWindowTitleMatches,
    ProcessNameMatches
}
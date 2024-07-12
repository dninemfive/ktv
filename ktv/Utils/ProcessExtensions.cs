using d9.utl;
using System.Diagnostics;

namespace d9.ktv;
internal static class ProcessExtensions
{
    private static ProcessModule? TryGetProcessModule(this Process process)
    {
        try
        {
            return process.MainModule;
        }
        catch
        {
            return null;
        }
    }
    internal static string? FileName(this Process process)
        => process.TryGetProcessModule()?.FileName;
    public static string FullInfo(this Process process)
    {
        string extraInfo = process.MainWindowTitle;
        if(process.FileName() is string s && !string.IsNullOrWhiteSpace(s))
        {
            if (!string.IsNullOrWhiteSpace(extraInfo))
                extraInfo += "/";
            extraInfo += s;
        }
        if (!string.IsNullOrWhiteSpace(extraInfo))
            extraInfo = $" ({extraInfo})";
        return $"{process.ProcessName}{extraInfo}";
    }
    internal static bool IsInFolder(this Process process, string folder)
        => process.FileName()?
                  .IsInFolder(folder)
            ?? false;
    internal static bool NameContains(this Process process, string name)
        => process.ProcessName.Contains(name, StringComparison.InvariantCultureIgnoreCase);
    internal static bool MainWindowTitleContains(this Process process, string name)
        => process.MainWindowTitle.Contains(name, StringComparison.InvariantCultureIgnoreCase);
}
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// https://stackoverflow.com/a/115905
namespace d9.ktv;

public static partial class ForegroundWindow
{
    [LibraryImport("user32.dll")]
    private static partial nint GetForegroundWindow();
    public static Process? Process
    {
        get
        {
            nint handle = GetForegroundWindow();
            foreach(Process process in Process.GetProcesses())
                if (process.MainWindowHandle == handle)
                    return process;
            return null;
        }
    }
}
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// https://stackoverflow.com/a/115905
namespace d9.ktv;

public static partial class ActiveWindow
{
    [LibraryImport("user32.dll")]
    private static partial nint GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    // http://www.jasinskionline.com/windowsapi/ref/g/getwindowtext.html
    private static extern int GetWindowText(nint hWnd, StringBuilder text, int count);
    public const int MaxLength = 512;
    public static nint Handle => GetForegroundWindow();
    public static string? Title
    {
        get
        {
            StringBuilder buffer = new(MaxLength);
            nint handle = GetForegroundWindow();
            if (GetWindowText(handle, buffer, MaxLength) > 0)
            {
                return buffer.ToString();
            }
            return null;
        }
    }
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
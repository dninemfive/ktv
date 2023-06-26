using System.Runtime.InteropServices;
using System.Text;

// https://stackoverflow.com/a/115905
namespace d9.ktv;

public static partial class ActiveWindow
{
    [LibraryImport("user32.dll")]
    private static partial IntPtr GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    public const int MaxLength = 256;
    public static string? Title
    {
        get
        {
            StringBuilder buffer = new(MaxLength);
            IntPtr handle = GetForegroundWindow();
            if (GetWindowText(handle, buffer, MaxLength) > 0)
                return buffer.ToString();
            return null;
        }
    }
    public static ActiveWindowInfo Info
    {
        get
        {
            if (Title is string s)
            {
                foreach (WindowNameParser wnp in WindowNameParser.List)
                {
                    if (wnp.Matches(s))
                    {
                        return wnp.Split(s);
                    }
                }
                return new(s, alias: true);
            }
            return ActiveWindowInfo.Invalid;
        }
    }
}
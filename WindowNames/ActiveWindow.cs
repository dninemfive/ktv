using System.Runtime.InteropServices;
using System.Text;

// https://stackoverflow.com/a/115905
namespace d9.ktv;

public static partial class ActiveWindow
{
    [LibraryImport("user32.dll")]
    private static partial IntPtr GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    // http://www.jasinskionline.com/windowsapi/ref/g/getwindowtext.html
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    [DllImport("kernel32.dll")]
    private static extern long GetLastError();
    public const int MaxLength = 255;
    public static string? Title
    {
        get
        {
            Console.WriteLine($"get_Title: make buffer");
            StringBuilder buffer = new(MaxLength + 1);
            Console.WriteLine($"get_Title: get foreground window handle");
            IntPtr handle = GetForegroundWindow();
            Console.WriteLine($"get_Title: try");
            try
            {
                Console.WriteLine($"(handle: {handle}, buffer: {buffer.Length}/{buffer.Capacity})");
                if (GetWindowText(handle, buffer, MaxLength) > 0)
                {
                    Console.WriteLine($"get_Title: return string");
                    return buffer.ToString();
                }
            } 
            catch(Exception e)
            {
                Console.WriteLine($"Exception follows:\n{e}");
                // Console.WriteLine($"Error code {GetLastError()}");
            }
            Console.WriteLine($"get_Title: return null");
            return null;
        }
    }
    public static ActiveWindowInfo Info
    {
        get
        {
            Console.WriteLine($"get_Info: get title");
            string? title = Title;
            Console.WriteLine("get_Info: cast title");
            if (title is string s)
            {
                Console.WriteLine($"get_Info: loop");
                foreach (Parser wnp in Parsers.All)
                {
                    Console.WriteLine($"get_Info: try {wnp}");
                    if (wnp.Try(title, out ActiveWindowInfo? result)) return result!;
                }
                Console.WriteLine($"get_Info: return string");
                return new(s, alias: true);
            }
            Console.WriteLine($"get_Info: return empty string");
            return new("");
        }
    }
}
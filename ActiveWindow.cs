using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

// https://stackoverflow.com/a/115905
namespace ktv
{
    public static class ActiveWindow
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        public const int MaxLength = 256;
        public static string? Title
        {
            get
            {
                StringBuilder buffer = new(MaxLength);
                IntPtr handle = GetForegroundWindow();
                if (GetWindowText(handle, buffer, MaxLength) > 0) return buffer.ToString();
                return null;
            }
        }
        public static (string app, string? details)? Info
        {
            get
            {
                if(Title?.SplitOn("-—", first: false) is (string, string) result)
                {
                    if (result.b is null) return (result.a, null);
                }
                return null;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal static class Utils
    {
        public static void Log(string message)
        {
            Console.Write(message);
            File.AppendAllText(ConsoleArgs.LogPath, message);
        }
        public static void DebugLog(object obj)
        {
            if (!ConsoleArgs.Debug) return;
            Log(obj.PrintNull());
        }
    }
}

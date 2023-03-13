using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    public delegate PossiblyNull<T> Parser<T>(string key);
    public static class ConsoleArgParsers
    {
        public static Parser<bool> Bool => key => bool.TryParse(key, out bool b) ? b : PossiblyNull<bool>.Null;
        public static Parser<float> Float => key => float.TryParse(key, out float f) ? f : PossiblyNull<float>.Null;
        public static Parser<int> Int => key => int.TryParse(key, out int i) ? i : PossiblyNull<int>.Null;
        public static Parser<DateTime> DateTime => key => System.DateTime.TryParse(key, out DateTime dt) ? dt : ktv.PossiblyNull<System.DateTime>.Null;
        public static Parser<string> DirectoryPath => _directoryPath;
        private static PossiblyNull<string> _directoryPath(string key)
        {
            string path;
            try
            {
                path = Path.GetFullPath(key);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Log folder was specified as {key}, but it is not a valid path: {e.Message}");
                return PossiblyNull<string>.Null;
            }
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            // https://stackoverflow.com/a/1395226
            if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                Console.WriteLine($"Log folder was specified as {key}, but it is a file, not a folder.");
                return PossiblyNull<string>.Null;
            }
            return path;
        }
    }
}

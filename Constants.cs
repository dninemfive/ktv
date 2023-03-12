using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    public static class Constants
    {
        public const int MillisecondsPerMinute = 60 * 1000;
        public static readonly string logPath = $"{DateTime.Now:yyyyMMddHHmmss}.ktv.log";
    }
}

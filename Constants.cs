﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    public static class Constants
    {
        public static readonly string LogName = $"{DateTime.Now.Format(TimeFormats.DateTime24H!)}.ktv.log";
    }
    public static class TimeFormats
    {        
        public const string Time = "h:mm tt";
        public const string Time24H = "HHmmss";
        public const string Date = "yyyyMMdd";
        public static readonly string DateTime24H = $"{Date}{Time24H}";
    }
}

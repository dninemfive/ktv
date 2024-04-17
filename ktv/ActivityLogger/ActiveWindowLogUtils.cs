﻿using d9.utl;

namespace d9.ktv;
public static class ActiveWindowLogUtils
{
    public static TimeSpan FileDuration => TimeSpan.FromMinutes(15);
    public static string FileNameFor(DateTime time)
    {
        time = time.Floor(TimeSpan.FromMinutes(15));
        string fileName = $"{time:yyyy'-'MM'-'dd' 'HH'-'mm'-'ss}.ktv.log".FileNameSafe();
        if (!File.Exists(fileName))
            File.AppendAllText(fileName, "");
        return fileName;
    }
    public static IEnumerable<string> FileNamesFor(DateTime start, DateTime end)
    {
        DateTime cur = start;
        while(cur < end)
        {
            yield return FileNameFor(cur);
            cur += FileDuration;
        }
    }
}

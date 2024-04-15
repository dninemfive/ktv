using d9.utl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public static class ProcessExpressions
{
    public static Expression Contains(string variableName, string expectedValue)
        => (assignment, children)
        => assignment[variableName] is string s && s.Contains(expectedValue);
    public static Expression IsIn(string variableName, string expectedValue)
        => (assignment, children)
        => assignment[variableName] is string s && s.IsInFolder(expectedValue);
    public static IReadOnlyDictionary<string, object?> Assignment(this Process process) => new Dictionary<string, object?>()
    {
        { "processName", process.ProcessName },
        { "fileName", process.FileName() },
        { "mainWindowTitle", process.MainWindowTitle }
    };
    public static bool ProcessExample(this Process process)
        => Or(process.Assignment(), Contains("mainWindowTitle", "Minecraft"), IsIn("fileName", "C:/Program Files (x86)/Steam"));

}

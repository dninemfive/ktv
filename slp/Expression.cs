using d9.slp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace d9.ktv;
/*
 * Goal: flexibly and serializably express conditions in a convenient way
 * e.g. for processes:  "(processName contains "minecraft") or (processFolder isIn "C:/Program Files (x86)/Steam")"
 *      for scheduling: "((time after 12:30AM) and (time before 10:00AM)) or not(day is Sunday)"
 */
public class Assignment<T>(IReadOnlyDictionary<string, T?> dict)
{
    private IReadOnlyDictionary<string, T?> _dict = dict;
    public T? this[string key] => _dict.TryGetValue(key, out T? result) ? result : default;
    public static implicit operator Assignment<T>(IReadOnlyDictionary<string, T?> dict)
        => new(dict);
}
public abstract class Expression<T>
{
    public abstract bool Matches(Assignment<T> assignment);
}
public class ProcessExpression : Expression<string>
{
    public static Assignment<string> AssignmentFor(Process process) => new Dictionary<string, string?>()
    {
        { "processName", process.ProcessName },
        { "fileName", process.FileName() },

    };
}
using d9.slp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace d9.ktv.slp;
/*
 * Goal: flexibly and serializably express conditions in a convenient way
 * e.g. for processes:  "(processName contains "minecraft") or (processFolder isIn "C:/Program Files (x86)/Steam")"
 *      for scheduling: "((time after 12:30AM) and (time before 10:00AM)) or not(day is Sunday)"
 */
public class ProcessExpression
{
    public Func<Process, string> Selector;
    public Func<string, string, bool> Operand;
    public string Value;
    public bool Evaluate(Process process) => Operand(Selector(process), Value);
}
public class ProcessExpressionExpression
{
    public ProcessExpression[] Expressions;
    public Func<bool[], bool> Operand;
}
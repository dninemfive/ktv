using d9.slp;
using d9.utl;
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
public delegate bool Expression(IReadOnlyDictionary<string, object?> assignment, params Expression[] children);
public static class Expressions
{
    public static bool Not(IReadOnlyDictionary<string, object?> assignment, params Expression[] children)
    {
        if (children.Length != 1)
            throw new ArgumentException($"Not expression must have exactly one child!", nameof(children));
        return !children.First()(assignment);
    }
    private static bool CollectionExpressionBase(IReadOnlyDictionary<string, object?> assignment, IEnumerable<Expression> children, bool successValue, bool anySucceeded)
    {
        foreach (Expression child in children)
            if (child(assignment) == successValue)
                return anySucceeded;
        return !anySucceeded;
    }
    public static bool Or(IReadOnlyDictionary<string, object?> assignment, params Expression[] children)
        => CollectionExpressionBase(assignment, children, true, true);
    public static bool And(IReadOnlyDictionary<string, object?> assignment, params Expression[] children)
        => CollectionExpressionBase(assignment, children, false, false);
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
    public static Expression IsBefore(string variableName, TimeOnly expectedValue)
        => (assignment, children)
        => assignment[variableName] is DateTime dt && TimeOnly.FromDateTime(dt) < expectedValue;
    public static Expression IsAfter(string variableName, TimeOnly expectedValue)
        => (assignment, children)
        => assignment[variableName] is DateTime dt && TimeOnly.FromDateTime(dt) > expectedValue;
    public static Expression IsBetween(string variableName, TimeOnly start, TimeOnly end)
        => (assignment, children)
        => And(assignment, IsAfter(variableName, start), IsBefore(variableName, end));
    public static Expression IsOn(string variableName, DayOfWeek dayOfWeek)
        => (assignment, children)
        => assignment[variableName] is DateTime dt && dt.DayOfWeek == dayOfWeek;
    public static IReadOnlyDictionary<string, object?> Assignment(this DateTime dt) => new Dictionary<string, object?>() { { "value", dt } };
    public static bool DateTimeExample(this DateTime dt)
        => Or(dt.Assignment(), IsBetween("value", new(0, 30), new(10, 0)), IsOn("value", DayOfWeek.Sunday));
}
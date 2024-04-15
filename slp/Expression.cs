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
public abstract class Expression
{
    public abstract bool Evaluate(IReadOnlyDictionary<string, object?> assignment, params Expression[] children);
}
public class NotExpression : Expression
{
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment, params Expression[] children)
    {
        if (children.Length != 1)
            throw new ArgumentException($"Not expression must have exactly one child!", nameof(children));
        return !children.First().Evaluate(assignment);
    }
}
public class OrExpression : Expression
{
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment, params Expression[] children)
        => children.EvaluateCollection(assignment, true, true);
}
public class AndExpression : Expression
{
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment, params Expression[] children)
        => children.EvaluateCollection(assignment, false, false);
}
public static class ExpressionExtensions
{
    public static bool EvaluateCollection(this IEnumerable<Expression> children, IReadOnlyDictionary<string, object?> assignment, bool successValue, bool anySucceeded)
    {
        foreach (Expression child in children)
            if (child.Evaluate(assignment) == successValue)
                return anySucceeded;
        return !anySucceeded;
    }
}
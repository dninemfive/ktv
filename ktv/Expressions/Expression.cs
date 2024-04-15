using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public abstract class Expression(params Expression[] children)
{
    public Expression[] Children { get; private set; } = children;
    public abstract bool Evaluate(IReadOnlyDictionary<string, object?> assignment);
}
public class NotExpression(params Expression[] children) : Expression(children)
{
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment)
        => Children.Length == 1 ? !Children[0].Evaluate(assignment) 
                                : throw new Exception($"Not requires exactly one child expression!");
}
public class OrExpression(params Expression[] children) : Expression(children)
{
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment)
        => assignment.CollectionExpressionBase(Children, true, true);
}
public class AndExpression(params Expression[] children) : Expression(children)
{
    public static AndExpression Create(params Expression[] children) => new(children);
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment)
        => assignment.CollectionExpressionBase(Children, false, false);
}
public class FunctionExpression(string variableName, Func<object?, bool> func) : Expression()
{
    public override bool Evaluate(IReadOnlyDictionary<string, object?> assignment)
        => assignment.TryGetValue(variableName, out object? result) && func(result);
}
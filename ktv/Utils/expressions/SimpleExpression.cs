using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv.Utils.expressions;
[AttributeUsage(AttributeTargets.Method)]
public class ExpressionOperatorAttribute(string name, Type inputType, Type outputType) : Attribute
{
    public string Name { get; private set; } = name;
    public Type InputType { get; private set; } = inputType;
    public Type OutputType { get; private set; } = outputType;
}
public class ExpressionTree
{

}
// desired expressions:
// Not(Expression<bool>) -> bool
//      (NOT expr)
// Or(Expression<bool>[]) -> bool
//      (expr OR expr)
// And(Expression<bool>[]) -> bool
//      (expr AND expr)
// IsInFolder(string, string) -> bool
//      (expr INFOLDER expr)
// RegexMatches(string, string) -> bool
//      (expr MATCHES expr)
// Get(string) -> object?
//      (expr GET)
public abstract class Expression
{
    public ExpressionResultMode Mode { get; set; }
    public bool Apply(Assignment assignment);
}
public class ExpressionSequence
{
    public IEnumerable<Expression> Children { get; set; }
    public bool Apply();
}
public enum ExpressionResultMode
{
    Standard,
    Invert,     
    Fail,
    Succeed
}
public static class ExpressionExtensions
{
    public static bool ToResult(this bool input, ExpressionResultMode mode)
        => mode switch
        {
            ExpressionResultMode.Standard => input,
            ExpressionResultMode.Invert => !input,
            ExpressionResultMode.Fail => false,
            ExpressionResultMode.Succeed => true,
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
}
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
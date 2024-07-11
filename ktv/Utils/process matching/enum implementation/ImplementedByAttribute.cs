namespace d9.ktv;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ImplementedByAttribute(Type? type, string methodName) : Attribute
{
    public ImplementedByAttribute(string methodName) : this(null, methodName) { }
    public readonly Type? Type = type;
    public readonly string MethodName = methodName;
}
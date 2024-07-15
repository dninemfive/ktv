namespace d9.ktv;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ImplementedByAttribute(string methodName) : Attribute
{
    public readonly string MethodName = methodName;
}
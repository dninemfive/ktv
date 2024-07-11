using System.Reflection;

namespace d9.ktv;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class ImplementationMethodAttribute(string methodName) : Attribute
{
    public readonly string MethodName = methodName;
}
internal class EnumImplementation<E, D>
    where E : struct, Enum
    where D : Delegate
{
    private readonly Dictionary<E, D> _dict = new();
    internal EnumImplementation()
    {
        Type enumType = typeof(E), thisType = GetType();
        foreach(string fieldName in Enum.GetNames(enumType))
        {
            ImplementationMethodAttribute? attr = enumType.GetField(fieldName)!.GetCustomAttribute<ImplementationMethodAttribute>();
            string implementingMethodName = attr?.MethodName ?? fieldName;
            MethodInfo implementingMethod = thisType.GetMethod(implementingMethodName, BindingFlags.Static)
                ?? throw new Exception($"Could not find method {thisType.Name}.{implementingMethodName}(), meant to implement {enumType.Name}.{fieldName}!");
            _dict.Add(Enum.Parse<E>(fieldName), implementingMethod.CreateDelegate<D>());
        }
    }
    public D this[E key] => _dict[key];
}
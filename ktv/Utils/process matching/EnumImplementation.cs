using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class EnumImplementation<E, D>
    where E : struct, Enum
    where D : Delegate
{
    private readonly Dictionary<E, D> _dict = new();
    public EnumImplementation(Type? defaultType = null)
    {
        Type enumType = typeof(E), thisType = GetType();
        foreach (string fieldName in Enum.GetNames(enumType))
        {
            ImplementedByAttribute? attr = enumType.GetField(fieldName)!.GetCustomAttribute<ImplementedByAttribute>();
            string implementingMethodName = attr?.MethodName ?? fieldName;
            Type implementingType = attr?.Type ?? defaultType ?? thisType;
            MethodInfo implementingMethod = implementingType.GetMethod(implementingMethodName, BindingFlags.Static)
                ?? throw new Exception($"Could not find method {implementingType.Name}.{implementingMethodName}(), meant to implement {enumType.Name}.{fieldName}!");
            _dict.Add(Enum.Parse<E>(fieldName), implementingMethod.CreateDelegate<D>());
        }
    }
    public D this[E key] => _dict[key];
}

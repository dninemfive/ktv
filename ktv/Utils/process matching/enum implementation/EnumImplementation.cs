using System.Reflection;

namespace d9.ktv;
public class EnumImplementation<E, D>
    where E : struct, Enum
    where D : Delegate
{
    private readonly Dictionary<E, D> _dict = new();
    public EnumImplementation(Type? defaultType = null)
    {
        Console.WriteLine($"Initializing implementation of enum {typeof(E).Name}...");
        Type enumType = typeof(E), thisType = GetType();
        foreach (string fieldName in Enum.GetNames(enumType))
        {
            ImplementedByAttribute? attr = enumType.GetField(fieldName)!.GetCustomAttribute<ImplementedByAttribute>();
            string implementingMethodName = attr?.MethodName ?? fieldName;
            Type implementingType = attr?.Type ?? defaultType ?? thisType;
            MethodInfo implementingMethod = implementingType.GetMethod(implementingMethodName, BindingFlags.Static | BindingFlags.Public)
                ?? throw new Exception($"Could not find method {implementingType.Name}.{implementingMethodName}(), meant to implement {enumType.Name}.{fieldName}!");
            Console.WriteLine($"\t{fieldName,-24}\t{implementingType.Name}.{implementingMethodName}()");
            _dict.Add(Enum.Parse<E>(fieldName), implementingMethod.CreateDelegate<D>());
        }
    }
    public D this[E key] => _dict[key];
}

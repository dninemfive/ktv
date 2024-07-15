using System.Reflection;

namespace d9.ktv;
public abstract class EnumImplementation<E, D>
    where E : struct, Enum
    where D : Delegate
{
    private readonly Dictionary<E, D> _dict = new();
    protected EnumImplementation()
    {
        Console.WriteLine($"Initializing implementation of enum {typeof(E).Name}...");
        Type enumType = typeof(E), implementingType = GetType();
        foreach (string fieldName in Enum.GetNames(enumType))
        {
            ImplementedByAttribute? attr = enumType.GetField(fieldName)!.GetCustomAttribute<ImplementedByAttribute>();
            string implementingMethodName = attr?.MethodName ?? fieldName;
            MethodInfo implementingMethod = implementingType.GetMethod(implementingMethodName, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new Exception($"Could not find method {implementingType.Name}.{implementingMethodName}(), meant to implement {enumType.Name}.{fieldName}!");
            Console.WriteLine($"\t{fieldName,-24}\t{implementingType.Name}.{implementingMethodName}()");
            _dict.Add(Enum.Parse<E>(fieldName), implementingMethod.CreateDelegate<D>());
        }
    }
    public D this[E key] => _dict[key];
}

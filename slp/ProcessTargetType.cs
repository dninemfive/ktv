using d9.ktv;
using System.Diagnostics;
using System.Reflection;

namespace d9.slp;
[AttributeUsage(AttributeTargets.Enum)]
internal class ImplementationSignatureAttribute(params Type[] types) : Attribute
{
    Type[] Signature = types;
}
[AttributeUsage(AttributeTargets.Field)]
internal class EnumImplementationAttribute(Type implementingType, string methodName) : Attribute
{
    public Type ImplementingType = implementingType;
    public string MethodName = methodName;
}
[ImplementationSignature(typeof(Process), typeof(string))]
public enum ProcessTargetType
{
    [EnumImplementation(typeof(ProcessExtensions), nameof(ProcessExtensions.MainWindowTitleContains))]
    MainWindowTitle,
    [EnumImplementation(typeof(ProcessExtensions), nameof(ProcessExtensions.NameContains))]
    ProcessName,
    [EnumImplementation(typeof(ProcessExtensions), nameof(ProcessExtensions.IsInFolder))]
    ProcessLocation
}
public delegate bool ProcessTargetTypeImplementation(Process process, string s);
public static class ProcessTargetTypeExtensions
{
    private static Dictionary<ProcessTargetType, ProcessTargetTypeImplementation> _dict = new();
    public static bool Matches(this ProcessTargetType processType, Process process, string s)
    {
        if (_dict.TryGetValue(processType, out ProcessTargetTypeImplementation? ptti))
            return ptti(process, s);
        // https://github.com/dotnet/runtime/discussions/81264
        EnumImplementationAttribute eia = typeof(ProcessTargetType).GetField(processType.ToString())?.GetCustomAttribute<EnumImplementationAttribute>()
                                        ?? throw new Exception($"Could not find an implementation for ProcessTargetType {processType}!");
        MethodInfo implementation = eia.ImplementingType.GetMethod(eia.MethodName)
                                        ?? throw new Exception($"Could not find implementing method {eia.ImplementingType}.{eia.MethodName} for ProcessTargetType {processType}!");
        ptti = (ProcessTargetTypeImplementation)implementation.Invoke(null);
    }
}
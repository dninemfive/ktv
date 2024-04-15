using d9.utl;
using System.Diagnostics;

namespace d9.ktv;
public static class ProcessExpressions
{
    private static ExpressionDelegate<AndExpression> And => Expressions.And;
    private static ExpressionDelegate<OrExpression> Or => Expressions.Or;
    public static Expression Contains(string variableName, string expectedValue)
        => variableName.Lambda(x => x is string s && s.Contains(expectedValue));
    public static Expression IsIn(string variableName, string expectedValue)
        => variableName.Lambda(x => x is string s && s.IsInFolder(expectedValue));
    public static IReadOnlyDictionary<string, object?> Assignment(this Process process) => new Dictionary<string, object?>()
    {
        { "processName", process.ProcessName },
        { "fileName", process.FileName() },
        { "mainWindowTitle", process.MainWindowTitle }
    };
    public static bool ProcessExample(this Process process)
        => Or(Contains("mainWindowTitle", "Minecraft"), IsIn("fileName", "C:/Program Files (x86)/Steam")).Evaluate(process.Assignment());

}

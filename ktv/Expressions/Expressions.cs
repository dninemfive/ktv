namespace d9.ktv;
/*
 * Goal: flexibly and serializably express conditions in a convenient way
 * e.g. for processes:  "(processName contains "minecraft") or (processFolder isIn "C:/Program Files (x86)/Steam")"
 *      for scheduling: "((time after 12:30AM) and (time before 10:00AM)) or not(day is Sunday)"
 */
public delegate T ExpressionDelegate<T>(params Expression[] children);
public static class Expressions
{
    public static AndExpression And(params Expression[] children)
        => new(children);
    public static OrExpression Or(params Expression[] children)
        => new(children);
    public static NotExpression Not(params Expression[] children)
        => new(children);
    public static bool CollectionExpressionBase(this IReadOnlyDictionary<string, object?> assignment, IEnumerable<Expression> children, bool successValue, bool anySucceeded)
    {
        foreach (Expression child in children)
            if (child.Evaluate(assignment) == successValue)
                return anySucceeded;
        return !anySucceeded;
    }
    public static FunctionExpression Lambda(this string variableName, Func<object?, bool> func)
        => new(variableName, func);
    public static Expression IsOfTypeAndEquals<T>(this string variableName, T value)
        where T : IEquatable<T>
        => variableName.Lambda((obj) => obj is T t && t.Equals(value));
    public static Expression IsOfTypeAndLessThan<T>(this string variableName, T value)
        where T : IComparable<T>
        => variableName.Lambda((obj) => obj is T t && t.CompareTo(value) < 0);
    public static Expression IsOfTypeAndGreaterThan<T>(this string variableName, T value)
        where T : IComparable<T>
        => variableName.Lambda((obj) => obj is T t && t.CompareTo(value) > 0);
    public static Expression IsOfTypeAndBetween<T>(this string variableName, T low, T high)
        where T : IComparable<T>
        => And(variableName.IsOfTypeAndGreaterThan(low), variableName.IsOfTypeAndLessThan(high));
}
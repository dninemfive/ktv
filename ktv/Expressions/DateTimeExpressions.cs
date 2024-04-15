namespace d9.ktv;
public static class DateTimeExpressions
{
    public static Expression IsBefore(string variableName, TimeOnly expectedValue)
        => variableName.IsOfTypeAndLessThan(expectedValue);
    public static Expression IsAfter(string variableName, TimeOnly expectedValue)
        => variableName.IsOfTypeAndGreaterThan(expectedValue);
    public static Expression IsBetween(string variableName, TimeOnly start, TimeOnly end)
        => variableName.IsOfTypeAndBetween(start, end);
    public static FunctionExpression IsOn(string variableName, DayOfWeek dayOfWeek)
        => new(variableName, (obj) => obj is DateTime dt && dt.DayOfWeek == dayOfWeek);
    public static IReadOnlyDictionary<string, object?> Assignment(this DateTime dt) => new Dictionary<string, object?>() { { "value", dt } };
    public static bool DateTimeExample(this DateTime dt)
        => Or(dt.Assignment(), IsBetween("value", new(0, 30), new(10, 0)), IsOn("value", DayOfWeek.Sunday));
}

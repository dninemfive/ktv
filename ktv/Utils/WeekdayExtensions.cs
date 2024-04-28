namespace d9.ktv;
public static class WeekdayExtensions
{
    public static string Abbreviation(this DayOfWeek dow)
        => dow.ToString()[0..2];
    public static string Abbreviation(this IEnumerable<DayOfWeek> dows)
        => dows.Distinct().Order().Select(Abbreviation).Aggregate((x, y) => $"{x}{y}");
    public static IEnumerable<DayOfWeek> ParseWeekdays(this string s)
    {
        s = s.ToLower();
        return s switch
        {
            "weekdays" or "weekends" => ParseWeekdayAlias(s),
            _ => ParseWeekdayString(s)
        };
    }
    private static IEnumerable<DayOfWeek> ParseWeekdayAlias(string alias)
        => alias switch
        {
            "weekdays" => [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday],
            "weekends" => [DayOfWeek.Saturday, DayOfWeek.Sunday],
            _ => throw new Exception($"incorrect weekday alias {alias} provided. this is a private method, how did you get here?")
        };
    private static IEnumerable<DayOfWeek> ParseWeekdayString(string s)
    {
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            string dayStr = day.ToString().ToLower();
            if (s.Contains(dayStr) || s.Contains(dayStr[0..2]))
                yield return day;
        }
    }
}
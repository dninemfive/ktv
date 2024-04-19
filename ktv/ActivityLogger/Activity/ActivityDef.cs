using System.Diagnostics;

namespace d9.ktv;
public class ActivityDef
{
    public required ProcessMatcher Matcher { get; set; }
    public string? FileNameRegex { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    public required string Pattern { get; set; }
    public string? Category { get; set; }
    public string? Name(Process? p)
    {
        if (!Matcher.Matches(p))
            return null;
        List<(string name, string? regex, string? value)> sourceVariables = [
            ("fileName", p.FileName(), FileNameRegex),
            ("mainWindowTitle", p.MainWindowTitle, MainWindowTitleRegex),
            ("processName", p.ProcessName, ProcessNameRegex)
        ];
        return Pattern.RegexReplace(sourceVariables);
    }
    public Activity? CreateActivityFrom(Process? p, ActivityConfig cfg)
    {
        if (!Matcher.Matches(p))
            return null;
        return new(Name(p)!, cfg.CategoryDefOf(Category));
    }
}
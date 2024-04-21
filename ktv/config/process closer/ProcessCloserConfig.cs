using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ProcessCloserConfig
{
    [JsonPropertyName("when")]
    public TimeConstraint? TimeConstraint { get; set; }
    [JsonPropertyName("match")]
    public ProcessMatcher? CloseProcesses { get; set; }
    [JsonPropertyName("except")]
    public ProcessMatcher? IgnoreProcesses { get; set; }
    public required float PeriodMinutes { get; set; }
    public bool ShouldClose(Process? p, DateTime dt)
    {
        if(CloseProcesses is null && IgnoreProcesses is null)
        {
            // never close all processes
            // maybe include an override switch but lol
            return false;
        }
        if (!(TimeConstraint?.Matches(dt) ?? false))
            return false;
        return !(IgnoreProcesses?.Matches(p) ?? false) && (CloseProcesses?.Matches(p) ?? true);
    }
}
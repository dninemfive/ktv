using System.Diagnostics;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ProcessCloserConfig
{
    [JsonPropertyName("when")]
    public TimeConstraint? TimeConstraint { get; set; }
    [JsonPropertyName("match")]
    public List<ProcessMatcherDef>? ProcessesToClose { get; set; }
    [JsonPropertyName("except")]
    public List<ProcessMatcherDef>? ProcessesToIgnore { get; set; }
    public required float PeriodMinutes { get; set; }
    public bool ShouldClose(Process? p, DateTime dt)
    {
        if(ProcessesToClose is null && ProcessesToIgnore is null)
        {
            // never close all processes
            // maybe include an override switch but lol
            return false;
        }
        if (!(TimeConstraint?.Matches(dt) ?? false))
            return false;
        return !ProcessesToIgnore.IsMatch(p) && ProcessesToClose.IsMatch(p);
    }
}
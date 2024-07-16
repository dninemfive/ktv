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
}
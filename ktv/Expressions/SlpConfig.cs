using System.Text.Json.Serialization;

namespace d9.ktv;
#pragma warning disable CS8618 // properties are initialized by JSON serialization
public class SlpConfig
{
    public static readonly SlpConfig Default = new()
    {
        StartTime = "12:30 AM",
        EndTime = "10:00 AM",
        MinutesBetweenCloseAttempts = 10,
        Close = [new(ProcessTargetType.ProcessLocation, @"C:\Program Files (x86)\Steam"),
                 new(ProcessTargetType.MainWindowTitle, "Minecraft"),
                 new(ProcessTargetType.MainWindowTitle, "Visual Studio")],
        Allow = [new(ProcessTargetType.ProcessName, "CrashHandler")]
    };
    [JsonInclude]
    public string StartTime { get; private set; }
    [JsonInclude]
    public string EndTime { get; private set; }
    [JsonInclude]
    public double MinutesBetweenCloseAttempts { get; private set; }
    [JsonInclude]
    public List<ProcessTargeter> Close { get; private set; }
    [JsonInclude]
    public List<ProcessTargeter> Allow { get; private set; }
}
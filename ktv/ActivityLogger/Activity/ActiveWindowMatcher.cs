using d9.utl;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ActiveWindowMatcher
{
    public class FileName
    {
        [JsonPropertyName("isInFolder")]
        public string? ParentFolder { get; set; }
        [JsonPropertyName("matchesRegex")]
        public string? Regex { get; set; }
        public bool Matches(string? fileName)
            => fileName is null
               || ((ParentFolder is null || fileName.IsInFolder(ParentFolder))
               && fileName.Matches(Regex));
    }
    [JsonPropertyName("fileName")]
    public FileName? FileNameMatcher { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    public bool Matches([NotNullWhen(true)] ActiveWindowLogEntry? awle)
        => awle is not null
           && (FileNameMatcher?.Matches(awle.FileName) ?? true)
           && (MainWindowTitleRegex is null || awle.MainWindowTitle.Matches(MainWindowTitleRegex))
           && (ProcessNameRegex is null || awle.ProcessName.Matches(ProcessNameRegex));
}
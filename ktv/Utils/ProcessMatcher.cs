using d9.utl;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace d9.ktv;
public class ProcessMatcher
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
        public override string ToString()
        {
            string folderMsg = $"is in `{ParentFolder}`", regexMsg = $"matches `{Regex}`";
            return $"filename {(ParentFolder is not null, Regex is not null) switch
            {
                (true, true)  => $"{folderMsg} and {regexMsg}",
                (true, false) => folderMsg,
                (false, true) => regexMsg,
                _ => "(not specified)"
            }}";
        }
    }
    [JsonPropertyName("fileName")]
    public FileName? FileNameMatcher { get; set; }
    [JsonPropertyName("mainWindowTitle")]
    public string? MainWindowTitleRegex { get; set; }
    [JsonPropertyName("processName")]
    public string? ProcessNameRegex { get; set; }
    public bool Matches(string? fileName, string? mainWindowTitle, string? processName)
        => (FileNameMatcher?.Matches(fileName) ?? true)
           && (MainWindowTitleRegex is null || mainWindowTitle.Matches(MainWindowTitleRegex))
           && (ProcessNameRegex is null || processName.Matches(ProcessNameRegex));
    public bool Matches([NotNullWhen(true)] Process? p)
        => p is not null && Matches(p.FileName(), p.MainWindowTitle, p.ProcessName);
    public bool Matches([NotNullWhen(true)] ActiveWindowLogEntry? awle)
        => awle is not null && Matches(awle.FileName, awle.MainWindowTitle, awle.ProcessName);
    public override string ToString()
    {
        List<(string name, string? value)> items = [
            ("", FileNameMatcher?.ToString()),
            ("mainWindowTitle matches", MainWindowTitleRegex),
            ("processName matches", ProcessNameRegex)
        ];
        return items.Where(x => x.value is not null)
                    .Select(x => $"{x.name} {x.value}")
                    .MultilineListWithAlignedTitle("process matches");
    }
}
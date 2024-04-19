using d9.utl.compat;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace d9.ktv;
public class Activity(string name, string category, string? eventId = null)
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public string? EventId { get; private set; } = eventId;
}
public class ProcessMatcher
{
    public class FileName
    {
        [JsonPropertyName("isInFolder")]
        public string? ParentFolder { get; set; }
        [JsonPropertyName("matchesRegex")]
        public string? MatchesRegex { get; set; }
        public bool Matches(string fileName) 
            => (ParentFolder is not null && Regex.IsMatch(fileName, ParentFolder)) 
            || (MatchesRegex is not null && Regex.IsMatch(fileName, MatchesRegex));
    }
    [JsonPropertyName("fileName")]
    public FileName? FileNameMatcher { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    
}
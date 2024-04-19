﻿using d9.utl;
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
    }
    [JsonPropertyName("fileName")]
    public FileName? FileNameMatcher { get; set; }
    public string? MainWindowTitleRegex { get; set; }
    public string? ProcessNameRegex { get; set; }
    public bool Matches([NotNullWhen(true)] Process? p)
        => p is not null
           && (FileNameMatcher?.Matches(p.FileName()) ?? true)
           && p.MainWindowTitle.Matches(MainWindowTitleRegex)
           && p.ProcessName.Matches(ProcessNameRegex);
}
using d9.utl;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace d9.ktv;
public class Activity(string name, string category, string? eventId = null)
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public string? EventId { get; private set; } = eventId;
}
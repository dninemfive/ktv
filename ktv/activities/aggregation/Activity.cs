using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;
using System.Diagnostics;

namespace d9.ktv;
public class Activity(string name, string category)
    : IEquatable<Activity>
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public static bool operator ==(Activity a, Activity b)
        => a.Name == b.Name && a.Category == b.Category;
    public static bool operator !=(Activity a, Activity b)
        => !(a == b);
    public override bool Equals(object? obj)
        => obj is Activity other && this == other;
    public bool Equals(Activity? other)
        => other is not null && this == other;
    public override int GetHashCode()
        => HashCode.Combine(Name, Category);
    public override string ToString()
        => $"[{Category}] {Name}";
    public Event ToEvent(DateTime start, DateTime end, GoogleUtils.EventColor color)
        => new()
        {
            Summary = Name,
            Description = $"{Category} (posted by ktv 2.0)",
            Start = start.Floor().ToEventDateTime(),
            End = end.Floor().ToEventDateTime(),
            ColorId = color.ToColorId()
        };
}
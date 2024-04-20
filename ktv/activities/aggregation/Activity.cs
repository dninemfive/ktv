using d9.utl.compat;

namespace d9.ktv;
public class Activity(string name, string category, GoogleUtils.EventColor? color)
    : IEquatable<Activity>
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public GoogleUtils.EventColor? Color { get; private set; } = color;
    public static bool operator ==(Activity a, Activity b)
        => a.Name == b.Name && a.Category == b.Category && a.Color == b.Color;
    public static bool operator !=(Activity a, Activity b)
        => !(a == b);
    public override bool Equals(object? obj)
        => obj is Activity other && this == other;
    public bool Equals(Activity? other)
        => other is not null && this == other;
    public override int GetHashCode()
        => HashCode.Combine(Name, Category, Color);
    public override string ToString()
        => $"[{Category}] {Name}";
    /*
public Event ToEvent(DateTime startTime, DateTime? endTime)
=> new()
{
  Summary = Name,
  Start = startTime.Round().ToEventDateTime(),
  EndTimeUnspecified = endTime is null,
  End = endTime?.Round().ToEventDateTime(),
  ColorId = ((int)Category.EventColor).ToString()
};*/
}
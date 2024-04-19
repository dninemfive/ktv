using d9.utl;
using d9.utl.compat;
using Google.Apis.Calendar.v3.Data;

namespace d9.ktv;
public class Activity(string name, string category, GoogleUtils.EventColor? color)
{
    public string Name { get; private set; } = name;
    public string Category { get; private set; } = category;
    public GoogleUtils.EventColor? Color { get; private set; } = color;
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
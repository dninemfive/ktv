namespace d9.ktv;
public class Activity
{
    public readonly string Name;
    public readonly string? Description;
    public string? EventId { get; private set; } = null;
    public readonly DateTime Start;
    private DateTime _end;
    public bool WasPosted => EventId is not null;
    private readonly bool _makeEvent;
    public DateTime End
    {
        get => _end;
        set
        {
            _end = value;
            if(_makeEvent) EventId = KtvConfig.PostOrUpdateEvent(Name, Start, End, EventId);
        }
    }
    public Activity(string name, DateTime start, DateTime end, string? description = null, bool makeEvent = false)
    {
        Name = name;
        Start = start;
        End = end;
        _makeEvent = makeEvent;
        Description = description;
    }
    public override string ToString() => $"{Start} - {End}: {Name}{(!string.IsNullOrEmpty(Description) ? $" ({Description})" : "")}";
}
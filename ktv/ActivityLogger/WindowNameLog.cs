namespace d9.ktv;
public static class WindowNameLog
{
    public class Entry(DateTime timestamp, string windowName) : IComparable<Entry>
    {
        public DateTime Timestamp { get; private set; } = timestamp;
        public string WindowName { get; private set; } = windowName;

        public int CompareTo(Entry? other) => Timestamp.CompareTo(other?.Timestamp);
        public override string ToString() => $"Entry({WindowName}, {Timestamp})";
    }
    private static readonly SortedSet<Entry> _rawData = [];
    static WindowNameLog()
    {
        foreach (Entry entry in FileManager.LoadEntries())
            _ = _rawData.Add(entry);
    }
    public static void Log(string s)
    {
        Entry entry = new(DateTime.Now, s);
        _ = _rawData.Add(entry);
        FileManager.Append(entry);
    }
    public static IEnumerable<Entry> EntriesBetween(DateTime start, DateTime end)
        => _rawData.SkipWhile(x => x.Timestamp < start).TakeWhile(x => x.Timestamp < end);
}

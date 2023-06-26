namespace d9.ktv;
public static class WindowNameLog
{
    public class Entry : IComparable<Entry>
    {
        public DateTime Timestamp { get; private set; }
        public string WindowName { get; private set; }
        public Entry(DateTime timestamp, string windowName)
        {
            Timestamp = timestamp;
            WindowName = windowName;
        }
        public int CompareTo(Entry? other) => Timestamp.CompareTo(other?.Timestamp);
    }
    private static readonly SortedSet<Entry> _rawData = new();
    public static void Log(string s) => _rawData.Add(new(DateTime.Now, s));
    public static IEnumerable<Entry> EntriesBetween(DateTime start, DateTime end)
        => _rawData.SkipWhile(x => x.Timestamp < start).TakeWhile(x => x.Timestamp < end);
}

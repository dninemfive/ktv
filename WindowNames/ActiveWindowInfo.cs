namespace d9.ktv;

public record ActiveWindowInfo
{
    public string Program { get; private set; } = "";
    public string? Details { get; private set; } = null;
    public ActiveWindowInfo(string program, string? details = null, bool alias = false)
    {
        Program = alias ? Alias.BestReplacement(program) : program;
        Details = details;
    }
    public ActiveWindowInfo((string program, string? details) tuple, bool alias = true) : this(tuple.program, tuple.details, alias) { }
    public override string ToString()
    {
        if (Details is not null)
        {
            return $"{Program,-30}\t{Details}";
        }
        return Program;
    }
    public static implicit operator ActiveWindowInfo((string program, string? details) tuple) => new(tuple, true);
}

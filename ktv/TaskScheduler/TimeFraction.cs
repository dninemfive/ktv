namespace d9.ktv;
public readonly struct TimeFraction(TimeSpan value, TimeSpan maximum)
{
    public readonly TimeSpan Value = value;
    public readonly TimeSpan Maximum = maximum;
    public void Deconstruct(out TimeSpan value, out TimeSpan maximum)
    {
        value = Value;
        maximum = Maximum;
    }
}

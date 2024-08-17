using d9.utl;
using d9.utl.compat.google;
using HueApi.ColorConverters.HSB;

namespace d9.ktv;
public class HueSynchronizer
    : TaskScheduler
{
    public HSB CurrentColor { get; private set; } = new(0, 0, 255);
    public readonly KtvService ParentService;
    public HueSynchronizer(Log log, KtvService parentService, Progress<TimeFraction?>? updateProgress = null)
        : base(log, updateProgress)
    {
        ParentService = parentService;
        parentService.Schedulers.OfType<ActiveWindowLogger>().First().OnActiveWindowLogged += UpdateCurrentColor;
    }
    public override Task<TaskScheduler> NextTask(DateTime time)
    {
        // note to self: if monitor off, progressively dim light
    }
    // see https://www.desmos.com/calculator/fiyegizqro
    // hardcoded for now, want to make configurable later
    public double BrightnessFor(TimeOnly time, TimeOnly start, TimeOnly end)
    {
        double period = (start - end).TotalHours;
        double currentHour = (time - start).TotalHours;
        return Math.Pow(Math.Sin((currentHour % 24) / period), 1.0 / 3.0);
    }
    public void UpdateCurrentColor(object? sender, ProcessSummary process)
    {
        GoogleCalendar.EventColor? color = ParentService.Config.ActivityTracker?.AggregationConfig?.Categorize(process)?.EventColor;
        if(color is not null)
        {
            CurrentColor = CurrentColor.AverageToward(color.HexCode()., 0.9);
        }
    }
}
public class HueSynchronizationConfig
{

}
public static class HueExtensions
{
    public static HSB AverageHueToward(this HSB old, HSB @new, double alpha)
    {
        // hue is an angle, so we want to make sure it takes the shortest path from old to new
    }
    public static HSB WithBrightness(this HSB val, double brightness)
        => new(val.Hue, val.Saturation, (int)(brightness * 255));
}
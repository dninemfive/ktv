using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueApi;
using HueApi.ColorConverters;
using HueApi.ColorConverters.HSB;
using HueApi.Models;
using d9.utl.compat.google;

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
        GoogleCalendar.EventColor color = ParentService.Config.ActivityTracker.AggregationConfig.Categorize(process).
    }
}
public class HueSynchronizationConfig
{

}
public static class HueExtensions
{
    public static HSB AverageToward(this HSB old, HSB @new, double alpha)
    {
        int avg(int a, int b)
            => (int)(alpha * a + (1 - alpha) * b);
        return new(avg(old.Hue, @new.Hue), avg(old.Saturation, @new.Saturation), avg(old.Brightness, @new.Brightness));
    }
    public static HSB WithBrightness(this HSB val, double brightness)
        => new(val.Hue, val.Saturation, (int)(brightness * 255));
}
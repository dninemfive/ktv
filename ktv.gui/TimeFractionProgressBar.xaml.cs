using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace d9.ktv.gui;
/// <summary>
/// Interaction logic for TimeFractionProgressBar.xaml
/// </summary>
public partial class TimeFractionProgressBar : UserControl
{
    public bool InProgress
    {
        get => ProgressBar.IsIndeterminate;
        set => ProgressBar.IsIndeterminate = value;
    }
    private TimeSpan? _timeUntilUpdate;
    public TimeSpan? TimeUntilUpdate
    {
        get => _timeUntilUpdate;
        set
        {
            _timeUntilUpdate = value;
            InProgress = _timeUntilUpdate is not null;
            if (_timeUntilUpdate.DivideBy(TimePerUpdate) is double d)
                ProgressBar.Value = d;
            UpdateLabels();
        }
    }
    private TimeSpan? _timePerUpdate;
    public TimeSpan? TimePerUpdate
    {
        get => _timePerUpdate;
        private set => _timePerUpdate = value;
    }
    public TimeFractionProgressBar()
    {
        InitializeComponent();
    }
    public void ReceiveUpdate(object? _, TimeFraction? tf)
    {
        if (tf is TimeFraction fraction)
        {
            (TimeUntilUpdate!, TimePerUpdate!) = fraction;
        }
        else
        {
            TimeUntilUpdate = null;
        }
    }
    private void UpdateLabels()
    {
        TimeSpan? elapsed = TimePerUpdate.Subtract(TimeUntilUpdate);
        Update(TimeElapsedLabel, elapsed, elapsed is null);
        Update(TimePerUpdateLabel, TimePerUpdate, elapsed is null);
    }
    private void Update(Label label, TimeSpan? value, bool? eitherNull = null)
    {
        eitherNull ??= (TimeUntilUpdate is null || TimePerUpdate is null);
        label.Visibility = eitherNull.Value ? Visibility.Hidden : Visibility.Visible;
        label.Content = value.ToStringOrNA();
    }
}
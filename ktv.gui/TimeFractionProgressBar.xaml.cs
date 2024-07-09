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
            if (_timeUntilUpdate is TimeSpan tuu && TimePerUpdate is TimeSpan tpu)
            {
                ProgressBar.Value = tuu / tpu;
                TimeElapsedLabel.Content = $"{tpu - tuu:g}";
            }
            else
            {
                TimeElapsedLabel.Content = "(N/A)";
            }
        }
    }
    private TimeSpan? _timePerUpdate;
    public TimeSpan? TimePerUpdate
    {
        get => _timePerUpdate;
        set
        {
            _timePerUpdate = value;
            TimePerUpdateLabel.Content = _timePerUpdate.ToStringOrNA();
        }
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
}
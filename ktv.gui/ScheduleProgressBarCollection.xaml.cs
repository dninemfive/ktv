using System;
using System.Collections.Generic;
using System.Data;
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
/// Interaction logic for ScheduleProgressBarCollection.xaml
/// </summary>
public partial class ScheduleProgressBarCollection : UserControl
{
    public TimeSpan UpdatePeriod { get; set; } = TimeSpan.FromSeconds(1);
    public ScheduleProgressBarCollection()
    {
        InitializeComponent();
    }
    public void Add(TaskScheduler scheduler)
    {
        TimeFractionProgressBar progressBar = new();
        scheduler.UpdateProgress.ProgressChanged += progressBar.ReceiveUpdate;
        RootPanel.Children.Add(progressBar);
    }
    public void Tick()
    {
        foreach(object? child in RootPanel.Children)
        {
            if (child is TimeFractionProgressBar tfpb && tfpb.TimeUntilUpdate is TimeSpan ts)
            {
                TimeSpan updatedTime = ts + UpdatePeriod;
                tfpb.TimeUntilUpdate = updatedTime;
            }
        }
    }
}

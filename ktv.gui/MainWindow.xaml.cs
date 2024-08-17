using d9.utl;
using d9.utl.compat;
using System.Text;
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
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Log Log { get; private set; }
    public KtvService? Service { get; private set; }
    public MainWindow()
    {
        InitializeComponent();
        Log = new(Log.Components.Console, Log.Components.WriteTextTo(DateTime.Now.GenerateLogFile()));
    }

    private async void StartButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is Button b)
            b.Visibility = Visibility.Hidden;
        string configPath = "config.json";
        KtvConfig? config = Config.TryLoad<KtvConfig>(configPath, out string? _);
        if (config is null)
        {
            await Log.WriteLine($"Could not find config at {configPath.AbsoluteOrInBaseFolder()}! Using default config...");
            config = KtvConfig.Default;
        }
        Service = await KtvService.CreateAndLog(config, Log);
        Task runningService = Service.Run();
        foreach (TaskScheduler scheduler in Service.Schedulers)
            ProgressBars.Add(scheduler);
        await runningService;
    }
}
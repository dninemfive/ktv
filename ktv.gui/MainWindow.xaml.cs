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
    private readonly Progress<string> _serviceLog;
    public KtvService Service { get; private set; }
    public MainWindow()
    {
        InitializeComponent();
        Log = new(DateTime.Now.GenerateLogFile(), Console, mode: Log.Mode.WriteImmediate);
        _serviceLog = new(Log.WriteLine);
        string configPath = "config.json";
        KtvConfig? config = Config.TryLoad<KtvConfig>(configPath);
        if (config is null)
        {
            Log.WriteLine($"Could not find config at {configPath.AbsolutePath()}! Using default config...");
            config = KtvConfig.Default;
        }
        Service = KtvService.CreateAndLog(config, _serviceLog);
    }

    private async void StartButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is Button b)
            b.Visibility = Visibility.Hidden;
        Task runningService = Service.Run();
        foreach (TaskScheduler scheduler in Service.Schedulers)
            ProgressBars.Add(scheduler);
        await runningService;
    }
}
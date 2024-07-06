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
    public MainWindow()
    {
        InitializeComponent();
        Log = new(DateTime.Now.GenerateLogFile(), Console, mode: Log.Mode.WriteImmediate);
    }
}
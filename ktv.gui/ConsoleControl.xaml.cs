using d9.utl;
using System.Windows.Controls;
using System.Windows.Documents;

namespace d9.ktv.gui;
/// <summary>
/// Interaction logic for ConsoleControl.xaml
/// </summary>
public partial class ConsoleControl : UserControl, IConsole
{
    public ConsoleControl()
    {
        InitializeComponent();
    }
    public Block? LastBlock
        => Output.Blocks.Any() ? Output.Blocks.LastBlock : null;
    public void Write(object? obj)
    {
        if(LastBlock is Paragraph last)
        {
            last.Inlines.Add(new Run($"{obj}"));
        } 
        else
        {
            WriteLine(obj);
        }
    }
    public void WriteLine(object? obj)
    {
        Output.Blocks.Add(obj.ToBlock());
    }
}

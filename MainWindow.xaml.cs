using System.Windows;
using WbStickers.Source.ViewModels;

namespace WbStickers;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new StickersViewModel();
    }
}
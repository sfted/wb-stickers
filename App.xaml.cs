using System.Text;
using System.Windows;

namespace WbStickers;

public partial class App : Application
{
    public App()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
using System.Windows;
using System.Windows.Controls;

namespace WbStickers.Source.Helpers;

// https://stackoverflow.com/questions/4202961
public class BrowserBehavior
{
    public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
        "Html", typeof(string), typeof(BrowserBehavior), new FrameworkPropertyMetadata(OnHtmlChanged));

    [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
    public static string GetHtml(WebBrowser d)
    {
        return (string)d.GetValue(HtmlProperty);
    }

    public static void SetHtml(WebBrowser d, string value)
    {
        d.SetValue(HtmlProperty, value);
    }

    static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        try
        {
            if (dependencyObject is WebBrowser browser)
                browser.NavigateToString(e.NewValue as string ?? "&nbsp;");
        }
        catch { }
    }
}
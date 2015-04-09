using System.Windows;
using System.Windows.Controls;

namespace Emby.WindowsPhone.Controls
{
    public class MediaBrowserLogo : Control
    {
        public static readonly DependencyProperty AltTextProperty =
            DependencyProperty.Register("AltText", typeof (string), typeof (MediaBrowserLogo), new PropertyMetadata(default(string)));

        public string AltText
        {
            get { return (string) GetValue(AltTextProperty); }
            set { SetValue(AltTextProperty, value); }
        }

        public MediaBrowserLogo()
        {
            DefaultStyleKey = typeof(MediaBrowserLogo);
        }
    }
}

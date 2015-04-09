using System.Windows;
using System.Windows.Controls;

namespace Emby.WindowsPhone.Controls
{
    public class EmbyLogo : Control
    {
        public static readonly DependencyProperty AltTextProperty =
            DependencyProperty.Register("AltText", typeof (string), typeof (EmbyLogo), new PropertyMetadata(default(string)));

        public string AltText
        {
            get { return (string) GetValue(AltTextProperty); }
            set { SetValue(AltTextProperty, value); }
        }

        public EmbyLogo()
        {
            DefaultStyleKey = typeof(EmbyLogo);
        }
    }
}

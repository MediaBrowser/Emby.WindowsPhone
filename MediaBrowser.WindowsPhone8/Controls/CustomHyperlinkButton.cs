using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Emby.WindowsPhone.Controls
{
    public class CustomHyperlinkButton : HyperlinkButton
    {
        public static readonly DependencyProperty TapCommandProperty = DependencyProperty.Register(
            "TapCommand", typeof (ICommand), typeof (CustomHyperlinkButton), new PropertyMetadata(default(ICommand)));

        public ICommand TapCommand
        {
            get { return (ICommand) GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public CustomHyperlinkButton()
        {
            Tap += OnTap;
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
            if (TapCommand != null)
            {
                TapCommand.Execute(CommandParameter);
            }
        }
    }
}

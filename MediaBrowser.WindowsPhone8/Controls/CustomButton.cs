using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaBrowser.WindowsPhone.Controls
{
    public class CustomButton : Button
    {
        public static readonly DependencyProperty TapCommandProperty = DependencyProperty.Register(
            "TapCommand", typeof (ICommand), typeof (CustomButton), new PropertyMetadata(default(ICommand)));

        public ICommand TapCommand
        {
            get { return (ICommand) GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public CustomButton()
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
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MediaBrowser.Windows8.SettingsViews
{
    public sealed partial class PushNotificationsSettings : UserControl
    {
        public PushNotificationsSettings()
        {
            this.InitializeComponent();
        }

        private void PushNotificationsSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.NotificationSettingsLoadedMsg));
        }
    }
}

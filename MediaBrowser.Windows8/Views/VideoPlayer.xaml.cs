using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Windows8.Model;
using Microsoft.PlayerFramework;
using Windows.UI.Xaml;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class VideoPlayer : MediaBrowser.Windows8.Common.LayoutAwarePage
    {
        public VideoPlayer()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => Messenger.Default.Send(new NotificationMessage(Constants.VideoPlayerLoadedMsg));

            //(new HLSSource.HLSSourceAdapter()).RegisterHandlers("target_bitrate=1250000 user=1c22a826-ef45-40e1-aec7-af0219787cfd company=\"CN=Scott\" serial=6246-3c43-fc5c-e3f7-a66b-73a5-5778-5f52-7038-497a");
        }

        private void thePlayer_MediaEnded_1(object sender, MediaPlayerActionEventArgs e)
        {
            SimpleIoc.Default.GetInstance<NavigationService>().GoBack();
        }

        private void thePlayer_MediaFailed_1(object sender, ExceptionRoutedEventArgs e)
        {
            var s = (MediaPlayer) sender;
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            base.GoBack(sender, e);
            Messenger.Default.Send(new NotificationMessage(Constants.SendVideoTimeToServerMsg));
        }
    }
}

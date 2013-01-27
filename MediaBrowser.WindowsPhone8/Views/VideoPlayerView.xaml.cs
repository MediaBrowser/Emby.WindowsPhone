using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.PlayerFramework;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView : PhoneApplicationPage
    {
        // Constructor
        public VideoPlayerView()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void ThePlayer_MediaEnded(object sender, Microsoft.PlayerFramework.MediaPlayerActionEventArgs e)
        {
            var s = "";
        }

        private void ThePlayer_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            var s = "";
        }

        private void ThePlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            var s = "";
        }

        private void ThePlayer_OnMediaStarting(object sender, MediaPlayerDeferrableEventArgs e)
        {
            var s = "";
        }

        private void ThePlayer_OnMediaStarted(object sender, RoutedEventArgs e)
        {
            var s = "";
        }
    }
}
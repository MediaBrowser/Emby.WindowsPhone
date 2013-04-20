using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.PlayerFramework;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView : PhoneApplicationPage
    {
        private readonly ILog _logger;
        // Constructor
        public VideoPlayerView()
        {
            _logger = new WPLogger(typeof(VideoPlayerView));

            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void ThePlayerMediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.SendVideoTimeToServerMsg));
        }

        private void ThePlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            _logger.Log("Error playing media: " + e.ErrorException.Message, LogLevel.Error);
            _logger.Log(e.ErrorException.StackTrace, LogLevel.Error);
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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            var result = MessageBox.Show("Are you sure you want to exit the video player?", "Are you sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Messenger.Default.Send(new NotificationMessage(Constants.SendVideoTimeToServerMsg));
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
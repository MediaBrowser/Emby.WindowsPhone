using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.PlayerFramework;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView
    {
        // Constructor
        public VideoPlayerView()
        {
            InitializeComponent();
        }

        private void ThePlayerMediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.SendVideoTimeToServerMsg));
        }

        private void ThePlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Log.ErrorException("Error playing media: " + e.ErrorException.Message, e.ErrorException);
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
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.SendVideoTimeToServerMsg));
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
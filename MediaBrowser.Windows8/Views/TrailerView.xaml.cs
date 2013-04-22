using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Windows8.Common;
using MetroLog;
using Windows.Foundation;
using Windows.UI.Xaml;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TrailerView : LayoutAwarePage
    {
        private readonly ILogger _logger;

        public TrailerView()
        {
            _logger = LogManagerFactory.DefaultLogManager.GetLogger<TrailerView>();

            this.InitializeComponent();

            Loaded += (sender, args) => Messenger.Default.Send(new NotificationMessage(Constants.TrailerPageLoadedMsg));
        }

        private void PlayerCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Player.IsFullScreen)
            {
                Player.Width = e.NewSize.Width;
                Player.Height = e.NewSize.Height;
            }
        }

        private void Player_OnIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                var offset = Player.TransformToVisual(LayoutRoot).TransformPoint(new Point());
                CanvasMover.X = -offset.X;
                CanvasMover.Y = -offset.Y;
                Player.Height = Window.Current.Bounds.Height;
                Player.Width = Window.Current.Bounds.Width;
            }
            else
            {
                Player.Width = playerCanvas.Width;
                Player.Height = playerCanvas.Height;
                CanvasMover.X = 0;
                CanvasMover.Y = 0;
            }
        }

        private void Player_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            _logger.Error("Video failed to play: " + e.ErrorMessage);
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            base.GoBack(sender, e);
            Messenger.Default.Send(new NotificationMessage(Constants.LeftTrailerMsg));
        }
    }
}

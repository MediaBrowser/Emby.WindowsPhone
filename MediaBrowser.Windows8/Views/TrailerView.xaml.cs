using GalaSoft.MvvmLight.Messaging;
using Windows.Foundation;
using Windows.UI.Xaml;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TrailerView : MediaBrowser.Windows8.Common.LayoutAwarePage
    {
        public TrailerView()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => Messenger.Default.Send(new NotificationMessage(Constants.TrailerPageLoadedMsg));
        }

        private void PlayerCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!player.IsFullScreen)
            {
                player.Width = e.NewSize.Width;
                player.Height = e.NewSize.Height;
            }
        }

        private void Player_OnIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                var offset = player.TransformToVisual(LayoutRoot).TransformPoint(new Point());
                CanvasMover.X = -offset.X;
                CanvasMover.Y = -offset.Y;
                this.player.Height = Window.Current.Bounds.Height;
                this.player.Width = Window.Current.Bounds.Width;
            }
            else
            {
                this.player.Width = playerCanvas.Width;
                this.player.Height = playerCanvas.Height;
                CanvasMover.X = 0;
                CanvasMover.Y = 0;
            }
        }
    }
}

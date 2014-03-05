using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for NowPlayingView.
    /// </summary>
    public partial class NowPlayingView
    {
        /// <summary>
        /// Initializes a new instance of the NowPlayingView class.
        /// </summary>
        public NowPlayingView()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                var item = (DataContext as PlaylistViewModel).NowPlayingItem;

                var url = item.BackgroundImageUrl;

                if (!string.IsNullOrEmpty(url))
                {
                    GridForBackground.Background = new ImageBrush
                    {
                        Stretch = Stretch.UniformToFill,
                        Opacity = 0.2,
                        ImageSource = new BitmapImage(new Uri(url))
                    };
                }
            };
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if(e.NavigationMode == NavigationMode.Back) Messenger.Default.Send(new NotificationMessage(Constants.Messages.PlaylistPageLeftMsg));
            base.OnNavigatedFrom(e);
        }
    }
}
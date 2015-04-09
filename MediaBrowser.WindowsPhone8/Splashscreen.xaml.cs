using GalaSoft.MvvmLight.Messaging;
using Emby.WindowsPhone.Services;

namespace Emby.WindowsPhone
{
    /// <summary>
    /// Description for Splashscreen.
    /// </summary>
    public partial class Splashscreen
    {
        /// <summary>
        /// Initializes a new instance of the Splashscreen class.
        /// </summary>
        public Splashscreen()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TileService.Current.PinnedUrlQuery = NavigationContext.QueryString;
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.SplashAnimationFinishedMsg));
        }
    }
}
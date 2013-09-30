using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.Services;

namespace MediaBrowser.WindowsPhone
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
            Loaded += (sender, args) => VisualStateManager.GoToState(this, "IsLoaded", true);
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
        }

        private void LoadAnimation_Completed(object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.SplashAnimationFinishedMsg));
        }
    }
}
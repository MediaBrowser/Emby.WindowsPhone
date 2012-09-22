using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone
{
    /// <summary>
    /// Description for Splashscreen.
    /// </summary>
    public partial class Splashscreen : PhoneApplicationPage
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
        }

        private void LoadAnimation_Completed(object sender, EventArgs e)
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(Constants.SplashAnimationFinishedMsg));
        }
    }
}
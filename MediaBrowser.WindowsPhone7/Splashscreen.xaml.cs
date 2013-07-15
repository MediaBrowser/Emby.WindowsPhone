using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

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
            string action;
            if (NavigationContext.QueryString.TryGetValue("action", out action))
            {
                string name, id;
                if (NavigationContext.QueryString.TryGetValue("name", out name) &&
                    NavigationContext.QueryString.TryGetValue("id", out id))
                {
                    var navigationUrl = string.Format("/Views/{0}View.xaml?id={1}&name={2}", action, id, name);
                    App.Action = navigationUrl;
                }
            }
        }

        private void LoadAnimation_Completed(object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.SplashAnimationFinishedMsg));
        }
    }
}
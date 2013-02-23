using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Navigation;

namespace MediaBrowser.Windows8.Views
{
    public sealed partial class LoadingView
    {
        public LoadingView()
        {
            this.InitializeComponent();
            Loaded += (s, e) => Messenger.Default.Send<NotificationMessage>(new NotificationMessage(Constants.LoadingPageLoadedMsg));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode == NavigationMode.Back)
            {
                Frame.GoBack();
            }
        }
    }
}

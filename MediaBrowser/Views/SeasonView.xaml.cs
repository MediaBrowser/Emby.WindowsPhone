using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for SeasonView.
    /// </summary>
    public partial class SeasonView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the SeasonView class.
        /// </summary>
        public SeasonView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage(Constants.ClearEpisodesMsg));
            }
        }
    }
}
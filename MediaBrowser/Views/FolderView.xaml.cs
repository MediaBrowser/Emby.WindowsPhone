using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone.Tools;
using GalaSoft.MvvmLight.Messaging;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for FolderView.
    /// </summary>
    public partial class FolderView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the FolderView class.
        /// </summary>
        public FolderView()
        {
            InitializeComponent();
        }

        private void btnChangeGrouping_Click(object sender, System.EventArgs e)
        {
            new PhoneFlipMenu(
                new PhoneFlipMenuAction("name", () => Messenger.Default.Send<NotificationMessage>(new NotificationMessage("name", Constants.ChangeGroupingMsg))),
                new PhoneFlipMenuAction("production year", () => Messenger.Default.Send<NotificationMessage>(new NotificationMessage("production year",Constants.ChangeGroupingMsg))),
                new PhoneFlipMenuAction("genre", () => Messenger.Default.Send<NotificationMessage>(new NotificationMessage("genre", Constants.ChangeGroupingMsg)))).Show();
                //                                     ,
                //new PhoneFlipMenuAction("studio", () =>
                //                                      {
                //                                          Messenger.Default.Send<NotificationMessage>(
                //                                              new NotificationMessage("studio",
                //                                                                      Constants.ChangeGroupingMsg));
                //                                      })
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage(Constants.ClearFoldersMsg));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage(Constants.ClearFilmAndTvMsg));
            }
        }
    }
}
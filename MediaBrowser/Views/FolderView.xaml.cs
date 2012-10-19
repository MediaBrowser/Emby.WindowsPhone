using System.Windows.Navigation;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone.Tools;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Shared;

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
                new PhoneFlipMenuAction("name", () => Messenger.Default.Send(new NotificationMessage("name", Constants.ChangeGroupingMsg))),
                new PhoneFlipMenuAction("production year", () => Messenger.Default.Send(new NotificationMessage("production year",Constants.ChangeGroupingMsg))),
                new PhoneFlipMenuAction("genre", () => Messenger.Default.Send(new NotificationMessage("genre", Constants.ChangeGroupingMsg)))).Show();
                //                                     ,
                //new PhoneFlipMenuAction("studio", () =>
                //                                      {
                //                                          Messenger.Default.Send<NotificationMessage>(
                //                                              new NotificationMessage("studio",
                //                                                                      Constants.ChangeGroupingMsg));
                //                                      })
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<FolderViewModel>(GetType());
            }
            else if(e.NavigationMode == NavigationMode.New)
            {
                DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                                  {
                                      SelectedFolder = App.SelectedItem
                                  };
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                History.Current.AddHistoryItem(GetType(), DataContext);
            }
        }
    }
}
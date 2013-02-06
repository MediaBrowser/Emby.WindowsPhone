using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.DTO;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
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

            new AppBarPrompt(
                new AppBarPromptAction(AppResources.NameLabel, () => Messenger.Default.Send(new NotificationMessage("name", Constants.ChangeGroupingMsg))),
                new AppBarPromptAction(AppResources.ProductionYear, () => Messenger.Default.Send(new NotificationMessage("production year", Constants.ChangeGroupingMsg))),
                new AppBarPromptAction(AppResources.Genre, () => Messenger.Default.Send(new NotificationMessage("genre", Constants.ChangeGroupingMsg)))).Show();
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
                if (App.SelectedItem is DtoBaseItem)
                {
                    DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                                      {
                                          SelectedFolder = (DtoBaseItem) App.SelectedItem
                                      };
                }
                else if (App.SelectedItem is BaseItemPerson)
                {
                    DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                    {
                        SelectedPerson = (BaseItemPerson)App.SelectedItem
                    };
                }
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
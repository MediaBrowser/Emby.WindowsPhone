using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using GalaSoft.MvvmLight.Messaging;


namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for FolderView.
    /// </summary>
    public partial class FolderView
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
                new AppBarPromptAction(AppResources.NameLabel, () => Messenger.Default.Send(new NotificationMessage(GroupBy.Name, Constants.Messages.ChangeGroupingMsg))),
                new AppBarPromptAction(AppResources.ProductionYear, () => Messenger.Default.Send(new NotificationMessage(GroupBy.ProductionYear, Constants.Messages.ChangeGroupingMsg))),
                new AppBarPromptAction(AppResources.Genre, () => Messenger.Default.Send(new NotificationMessage(GroupBy.Genre, Constants.Messages.ChangeGroupingMsg)))).Show();
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
                App.SelectedItem = ((FolderViewModel) DataContext).SelectedFolder;
            }
            else if(e.NavigationMode == NavigationMode.New)
            {
                if (App.SelectedItem is BaseItemDto)
                {
                    DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                                      {
                                          SelectedFolder = (BaseItemDto) App.SelectedItem
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
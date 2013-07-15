using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Windows8.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MediaBrowser.Shared;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FolderView : MediaBrowser.Windows8.Common.LayoutAwarePage
    {
        public FolderView()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => Messenger.Default.Send(new NotificationMessage(Constants.Messages.FolderViewLoadedMsg));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<FolderViewModel>(GetType());
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                                  {
                                      SelectedCollection = e.Parameter is BaseItemDto ? (BaseItemDto) e.Parameter : null,
                                      SelectedPerson = e.Parameter is BaseItemPerson ? (BaseItemPerson) e.Parameter : null
                                  };
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                History.Current.AddHistoryItem(GetType(), DataContext);
            }
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<MainViewModel>().ItemClicked.Execute(e);
        }
    }
}

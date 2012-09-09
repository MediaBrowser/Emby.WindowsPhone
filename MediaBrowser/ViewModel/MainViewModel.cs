using GalaSoft.MvvmLight;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using MediaBrowser.Model.DTO;
using System.Threading.Tasks;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService NavService;
        private readonly ApiClient ApiClient;
        private bool hasLoaded;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ApiClient apiClient, INavigationService navService)
        {
            Folders = new ObservableCollection<DTOBaseItem>();
            RecentItems = new ObservableCollection<DTOBaseItem>();
            if (IsInDesignMode)
            {
                RecentItems.Add(new DTOBaseItem { Id = new Guid("2fc6f321b5f8bbe842fcd0eed089561d"), Name = "A Night To Remember" } );
            }
            else
            {
                ApiClient = apiClient;
                NavService = navService;                
                WireCommands();
                App.Settings.LoggedInUser = new DTOUser { Id = new Guid("5d1cf7fce25943b790d140095457a42b") };
                DummyFolder = new DTOBaseItem
                                  {
                                      Type = "folder",
                                      Name = "recent"
                                  };
            }
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !hasLoaded)
                {
                    ProgressIsVisible = true;
                    ProgressText = "Loading folders...";

                    bool folderLoaded = await GetFolders();

                    ProgressText = "Getting recent items...";

                    bool recentLoaded = await GetRecent();

                    hasLoaded = (folderLoaded && recentLoaded);
                    ProgressIsVisible = false;
                    hasLoaded = true;
                }
            });

            NavigateToPage = new RelayCommand<DTOBaseItem>(NavService.NavigateTopage);
        }

        private async Task<bool> GetRecent()
        {
            try
            {
                var recent = await ApiClient.GetRecentlyAddedItemsAsync(App.Settings.LoggedInUser.Id);
                RecentItems.Clear();
                recent.OrderBy(x => x.DateCreated).Take(6).ToList().ForEach(recentItem => RecentItems.Add(recentItem));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> GetFolders()
        {
            try
            {
                var item = await ApiClient.GetItemAsync(Guid.Empty, App.Settings.LoggedInUser.Id);
                Folders.Clear();
                item.Children.ToList().ForEach(folder => Folders.Add(folder));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // UI properties
        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand<DTOBaseItem> NavigateToPage { get; set; }
        public ObservableCollection<DTOBaseItem> Folders { get; set; }
        public ObservableCollection<DTOBaseItem> RecentItems { get; set; }
        public DTOBaseItem DummyFolder { get; set; }
    }
}
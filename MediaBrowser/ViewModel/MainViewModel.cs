using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Entities;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using MediaBrowser.Model.DTO;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using ScottIsAFool.WindowsPhone.IsolatedStorage;

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
        private readonly ExtendedApiClient ApiClient;
        private bool hasLoaded;
        private DtoBaseItem[] recentItems;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ExtendedApiClient apiClient, INavigationService navService)
        {
            ApiClient = apiClient;
            NavService = navService;
            Folders = new ObservableCollection<DtoBaseItem>();
            RecentItems = new ObservableCollection<DtoBaseItem>();
            FavouriteItems = new ObservableCollection<DtoBaseItem>();
            if (IsInDesignMode)
            {
                Folders.Add(new DtoBaseItem { Id = "78dbff5aa1c2101b98ebaf42b72a988d", Name = "Movies", RecentlyAddedUnPlayedItemCount = 2 });
                RecentItems.Add(new DtoBaseItem { Id = "2fc6f321b5f8bbe842fcd0eed089561d", Name = "A Night To Remember" });
            }
            else
            {
                WireCommands();
                WireMessages();
                DummyFolder = new DtoBaseItem
                {
                    Type = "folder",
                    Name = "recent"
                };
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<PropertyChangedMessage<object>>(this, async m =>
                                                                                 {
                                                                                     if (m.PropertyName.Equals("IncludeTrailersInRecent"))
                                                                                     {
                                                                                         await SortRecent(recentItems);
                                                                                     }
                                                                                 });
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                await GetEverything(false);
            });

            RefreshDataCommand = new RelayCommand(async () =>
            {
                await GetEverything(true);
            });

            ChangeProfileCommand = new RelayCommand(() =>
                                                        {
                                                            Reset();
                                                            NavService.NavigateToPage("/Views/ChooseProfileView.xaml");
                                                        });

            PinCollectionCommand = new RelayCommand<DtoBaseItem>(collection =>
                                                                     {
                                                                         string tileUrl;
                                                                         var existingTile = GetShellTile(collection, out tileUrl); 
                                                                         if (existingTile != default(ShellTile))
                                                                         {
                                                                             var result = MessageBox.Show("Are you sure you wish to unpin this tile?", "Are you sure?", MessageBoxButton.OKCancel);
                                                                             if (result == MessageBoxResult.OK)
                                                                             {
                                                                                 existingTile.Delete();
                                                                                 Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.CollectionPinnedMsg));
                                                                             }
                                                                             return;
                                                                         }
                                                                         
#if WP8
                                                                         var tileDate = new CycleTileData
                                                                                            {
                                                                                                Title = collection.Name,
                                                                                                CycleImages = new Collection<Uri>
                                                                                                                  {
                                                                                                                      new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
                                                                                                                      new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative)
                                                                                                                  },
                                                                                                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative)
                                                                                            };
                                                                         ShellTile.Create(new Uri(tileUrl, UriKind.Relative), tileDate, true);
#else

#endif
                                                                         Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.CollectionPinnedMsg));

                                                                     });

            NavigateToPage = new RelayCommand<DtoBaseItem>(NavService.NavigateToPage);

            NavigateToAPage = new RelayCommand<string>(NavService.NavigateToPage);
        }

        private static ShellTile GetShellTile(DtoBaseItem collection, out string url)
        {
            var tileUrl = string.Format(Constants.PhoneCollectionTileUrlFormat, collection.Id, collection.Name);
            url = tileUrl;
            return ShellTile.ActiveTiles.SingleOrDefault(x => x.NavigationUri.ToString() == tileUrl);
        }

        private void Reset()
        {
            App.Settings.LoggedInUser = null;
            App.Settings.PinCode = string.Empty;
            ISettings.DeleteValue(Constants.SelectedUserSetting);
            ISettings.DeleteValue(Constants.SelectedUserPinSetting);
            hasLoaded = false;
            Folders.Clear();
            RecentItems.Clear();
        }

        private async Task GetEverything(bool isRefresh)
        {
            if (NavService.IsNetworkAvailable
                && App.Settings.CheckHostAndPort()
                && (!hasLoaded || isRefresh))
            {

                ProgressIsVisible = true;
                ProgressText = "Loading folders...";

                bool folderLoaded = await GetFolders();

                ProgressText = "Getting recent items...";

                bool recentLoaded = await GetRecent();

                ProgressText = "Getting favourites...";

                bool favouritesLoaded = await GetFavouriteItems();

                hasLoaded = (folderLoaded && recentLoaded && favouritesLoaded);
                ProgressIsVisible = false;
                hasLoaded = true;
            }
        }

        private async Task<bool> GetFavouriteItems()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    Filters = new[] { ItemFilter.IsFavorite, },
                    Recursive = true
                };
                var items = await ApiClient.GetItemsAsync(query);
                if (items != null && items.Items != null)
                {
                    foreach (var item in items.Items.Take(6))
                    {
                        FavouriteItems.Add(item);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> GetRecent()
        {
            try
            {
                var query = new ItemQuery
                {
                    Filters = new[] { ItemFilter.IsRecentlyAdded, ItemFilter.IsNotFolder, },
                    UserId = App.Settings.LoggedInUser.Id,
                    Fields = new[]
                                                 {
                                                     ItemFields.SeriesInfo,
                                                     ItemFields.DateCreated,
                                                     ItemFields.UserData, 
                                                 },
                    Recursive = true
                };
                var items = await ApiClient.GetItemsAsync(query);
                recentItems = items.Items;
                await SortRecent(items.Items);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task SortRecent(DtoBaseItem[] items)
        {
            RecentItems.Clear();

            var recent = await Utils.SortRecentItems(items);
            recent.ForEach(recentItem => RecentItems.Add(recentItem));
        }

        private async Task<bool> GetFolders()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    Fields = new[] { ItemFields.ItemCounts, }
                };
                var item = await ApiClient.GetItemsAsync(query);
                Folders.Clear();

                item.Items.OrderByDescending(x => x.SortName).ToList().ForEach(folder => Folders.Add(folder));
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
        public RelayCommand ChangeProfileCommand { get; set; }
        public RelayCommand RefreshDataCommand { get; set; }
        public RelayCommand<DtoBaseItem> NavigateToPage { get; set; }
        public RelayCommand<string> NavigateToAPage { get; set; }
        public RelayCommand<DtoBaseItem> PinCollectionCommand { get; set; }
        public ObservableCollection<DtoBaseItem> Folders { get; set; }
        public ObservableCollection<DtoBaseItem> RecentItems { get; set; }
        public ObservableCollection<DtoBaseItem> FavouriteItems { get; set; }
        public DtoBaseItem DummyFolder { get; set; }
    }
}
using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using MediaBrowser.Model.Dto;
using System.Threading.Tasks;
using MediaBrowser.WindowsPhone.Resources;
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
        private BaseItemDto[] recentItems;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ExtendedApiClient apiClient, INavigationService navService)
        {
            ApiClient = apiClient;
            NavService = navService;
            Folders = new ObservableCollection<BaseItemDto>();
            RecentItems = new ObservableCollection<BaseItemDto>();
            FavouriteItems = new ObservableCollection<BaseItemDto>();
            if (IsInDesignMode)
            {
                Folders.Add(new BaseItemDto { Id = "78dbff5aa1c2101b98ebaf42b72a988d", Name = "Movies" });
                RecentItems.Add(new BaseItemDto { Id = "2fc6f321b5f8bbe842fcd0eed089561d", Name = "A Night To Remember" });
            }
            else
            {
                WireCommands();
                WireMessages();
                DummyFolder = new BaseItemDto
                {
                    Type = "folder",
                    Name = AppResources.Recent.ToLower()
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

            PinCollectionCommand = new RelayCommand<BaseItemDto>(collection =>
                                                                     {
                                                                         string tileUrl;
                                                                         var existingTile = GetShellTile(collection, out tileUrl); 
                                                                         if (existingTile != default(ShellTile))
                                                                         {
                                                                             var result = MessageBox.Show(AppResources.MessageBoxUnpinText, AppResources.MessageBoxHeaderAreYouSure, MessageBoxButton.OKCancel);
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
                                                                         var tileData = new StandardTileData
                                                                                            {
                                                                                                Title = collection.Name,
                                                                                                BackBackgroundImage = new Uri("/Images/Logo.png", UriKind.Relative)
                                                                                            };
                                                                         ShellTile.Create(new Uri(tileUrl, UriKind.Relative), tileData);
#endif
                                                                         Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.CollectionPinnedMsg));

                                                                     });

            NavigateToPage = new RelayCommand<BaseItemDto>(NavService.NavigateToPage);

            NavigateToAPage = new RelayCommand<string>(NavService.NavigateToPage);
        }

        private static ShellTile GetShellTile(BaseItemDto collection, out string url)
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
                ProgressText = AppResources.SysTrayLoadingCollections;

                bool folderLoaded = await GetFolders();

                ProgressText = AppResources.SysTrayGettingRecentItems;

                bool recentLoaded = await GetRecent();

                ProgressText = AppResources.SysTrayGettingFavourites;

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
                                                     ItemFields.ParentId, 
                                                     ItemFields.AudioInfo, 
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

        private async Task SortRecent(BaseItemDto[] items)
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
            catch (Exception ex)
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
        public RelayCommand<BaseItemDto> NavigateToPage { get; set; }
        public RelayCommand<string> NavigateToAPage { get; set; }
        public RelayCommand<BaseItemDto> PinCollectionCommand { get; set; }
        public ObservableCollection<BaseItemDto> Folders { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public ObservableCollection<BaseItemDto> FavouriteItems { get; set; }
        public BaseItemDto DummyFolder { get; set; }
    }
}
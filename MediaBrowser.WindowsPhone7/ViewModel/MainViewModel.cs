using System;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Notifications;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using MediaBrowser.Model.Dto;
using System.Threading.Tasks;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using ScottIsAFool.WindowsPhone.ViewModel;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

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
        private readonly INavigationService _navService;
        private readonly ExtendedApiClient _apiClient;
        private readonly IApplicationSettingsService _applicationSettings;
        private bool _hasLoaded;
        private BaseItemDto[] _recentItems;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ExtendedApiClient apiClient, INavigationService navService, IApplicationSettingsService applicationSettings)
        {
            _apiClient = apiClient;
            _navService = navService;
            _applicationSettings = applicationSettings;

            Folders = new ObservableCollection<BaseItemDto>();
            RecentItems = new ObservableCollection<BaseItemDto>();
            FavouriteItems = new ObservableCollection<BaseItemDto>();

            if (IsInDesignMode)
            {
                Folders.Add(new BaseItemDto {Id = "78dbff5aa1c2101b98ebaf42b72a988d", Name = "Movies"});
                RecentItems.Add(new BaseItemDto {Id = "2fc6f321b5f8bbe842fcd0eed089561d", Name = "A Night To Remember"});
            }
            else
            {
                WireCommands();
                DummyFolder = new BaseItemDto
                {
                    Type = "folder",
                    Name = AppResources.Recent.ToLower()
                };
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<PropertyChangedMessage<object>>(this, async m =>
            {
                if (m.PropertyName.Equals("IncludeTrailersInRecent"))
                {
                    await SortRecent(_recentItems);
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
                Log.Info("Signing out");

                Reset();

                _navService.NavigateTo("/Views/ChooseProfileView.xaml");
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
                        Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.Messages.CollectionPinnedMsg));
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
                Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.Messages.CollectionPinnedMsg));

            });

            PlayMovieCommand = new RelayCommand<BaseItemDto>(async item =>
            {
                Log.Info("Playing {0} [{1}]", item.Type, item.Name);
#if WP8
                Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.PlayVideoItemMsg));
                _navService.NavigateTo("/Views/VideoPlayerView.xaml");
#else
                var bounds = Application.Current.RootVisual.RenderSize;
                var query = new VideoStreamOptions
                {
                    ItemId = item.Id,
                    VideoCodec = VideoCodecs.H264,
                    OutputFileExtension = ".asf",
                    //Static = true,
                    AudioCodec = AudioCodecs.Mp3,
                    VideoBitRate = 1000000,
                    AudioBitRate = 128000,
                    MaxAudioChannels = 2,
                    //Profile = "baseline",
                    //Level = "3",
                    //FrameRate = 30,
                    MaxHeight = 480,// (int)bounds.Width,
                    MaxWidth = 800// (int)bounds.Height
                };
                var url = _apiClient.GetVideoStreamUrl(query);
                System.Diagnostics.Debug.WriteLine(url);
                Log.Info(url);

                try
                {
                    Log.Info("Telling the server about watching this video");
                    await _apiClient.ReportPlaybackStartAsync(item.Id, App.Settings.LoggedInUser.Id).ConfigureAwait(true);
                }
                catch (HttpException ex)
                {
                    Log.ErrorException("PlayMovieCommand", ex);
                }

                var mediaPlayerLauncher = new MediaPlayerLauncher
                {
                    Orientation = MediaPlayerOrientation.Landscape,
                    Media = new Uri(url, UriKind.Absolute),
                    Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop,
                    //Location = MediaLocationType.Data
                };
                mediaPlayerLauncher.Show();
#endif
            });

            NavigateToPage = new RelayCommand<BaseItemDto>(_navService.NavigateTo);

            NavigateToAPage = new RelayCommand<string>(_navService.NavigateTo);

            NavigateToNotificationsCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.NotifcationNavigationMsg));

                _navService.NavigateTo("/Views/NotificationsView.xaml");
            });
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
            
            _applicationSettings.Reset(Constants.Settings.SelectedUserSetting);
            _applicationSettings.Reset(Constants.Settings.SelectedUserPinSetting);
            _applicationSettings.Save();

            _hasLoaded = false;
            Folders.Clear();
            RecentItems.Clear();
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ResetAppMsg));
        }

        private async Task GetEverything(bool isRefresh)
        {
            if (_navService.IsNetworkAvailable
                && App.Settings.CheckHostAndPort()
                && (!_hasLoaded || isRefresh))
            {

                SetProgressBar(AppResources.SysTrayLoadingCollections);

                var folderLoaded = await GetFolders();

                SetProgressBar(AppResources.SysTrayGettingRecentItems);

                var recentLoaded = await GetRecent();

                SetProgressBar(AppResources.SysTrayGettingFavourites);

                var favouritesLoaded = await GetFavouriteItems();

                _hasLoaded = (folderLoaded && recentLoaded && favouritesLoaded);

                //SetProgressBar("Checking notifications...");

                //await GetNotificaitonsCount();

                SetProgressBar();
            }
        }

        private async Task GetNotificaitonsCount()
        {
            var query = new NotificationQuery
            {
                Limit = 5,
                StartIndex = 0,
                UserId = App.Settings.LoggedInUser.Id
            };
            var summary = await _apiClient.GetNotificationsSummary(App.Settings.LoggedInUser.Id);
            var notifications = await _apiClient.GetNotificationsAsync(query);
        }

        private async Task<bool> GetFavouriteItems()
        {
            try
            {
                FavouriteItems.Clear();
                Log.Info("Getting favourites for user [{0}]", App.Settings.LoggedInUser.Name);

                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    Filters = new[] {ItemFilter.IsFavorite},
                    Recursive = true
                };
                var items = await _apiClient.GetItemsAsync(query);
                if (items != null && items.Items != null)
                {
                    foreach (var item in items.Items.Take(6))
                    {
                        FavouriteItems.Add(item);
                    }
                }
                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetFavouriteItems()", ex);
                return false;
            }
        }

        private async Task<bool> GetRecent()
        {
            try
            {
                Log.Info("Getting most recent items");

                var query = new ItemQuery
                {
                    Filters = new[] {ItemFilter.IsRecentlyAdded, ItemFilter.IsNotFolder},
                    UserId = App.Settings.LoggedInUser.Id,
                    Fields = new[]
                    {
                        ItemFields.SeriesInfo,
                        ItemFields.DateCreated,
                        ItemFields.UserData,
                        ItemFields.ParentId,
                        ItemFields.AudioInfo
                    },
                    Recursive = true
                };
                var items = await _apiClient.GetItemsAsync(query);
                _recentItems = items.Items;
                await SortRecent(items.Items);
                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetRecent()", ex);
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
                Log.Info("Getting collections for [{0}] ({1})", App.Settings.LoggedInUser.Name, App.Settings.LoggedInUser.Id);

                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    Fields = new[] {ItemFields.ItemCounts},
                    SortOrder = SortOrder.Ascending,
                    SortBy = new[] {ItemSortBy.SortName}
                };

                var item = await _apiClient.GetItemsAsync(query);

                Folders.Clear();

                item.Items.OrderByDescending(x => x.SortName).ToList().ForEach(folder => Folders.Add(folder));

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetFolders()", ex);
                return false;
            }
        }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand ChangeProfileCommand { get; set; }
        public RelayCommand RefreshDataCommand { get; set; }
        public RelayCommand<BaseItemDto> NavigateToPage { get; set; }
        public RelayCommand<string> NavigateToAPage { get; set; }
        public RelayCommand NavigateToNotificationsCommand { get; set; }
        public RelayCommand<BaseItemDto> PinCollectionCommand { get; set; }
        public RelayCommand<BaseItemDto> PlayMovieCommand { get; set; }
        public ObservableCollection<BaseItemDto> Folders { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public ObservableCollection<BaseItemDto> FavouriteItems { get; set; }
        public BaseItemDto DummyFolder { get; set; }
    }
}
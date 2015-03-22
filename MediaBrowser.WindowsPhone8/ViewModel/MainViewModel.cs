using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ImageTools.Helpers;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Notifications;
using MediaBrowser.Model.Querying;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using MediaBrowser.Model.Dto;
using System.Threading.Tasks;
using MediaBrowser.WindowsPhone.CimbalinoToolkit.Tiles;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;

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
        private readonly IPlaybackManager _playbackManager;
        private bool _hasLoaded;
        private BaseItemDto[] _recentItems;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IConnectionManager connectionManager, INavigationService navigationService, IPlaybackManager playbackManager)
            : base(navigationService, connectionManager)
        {
            _playbackManager = playbackManager;
            Folders = new ObservableCollection<BaseItemDto>();
            RecentItems = new ObservableCollection<BaseItemDto>();
            FavouriteItems = new ObservableCollection<BaseItemDto>();
            InProgressItems = new ObservableCollection<BaseItemDto>();

            if (IsInDesignMode)
            {
                Folders.Add(new BaseItemDto {Id = "78dbff5aa1c2101b98ebaf42b72a988d", Name = "Movies", UserData = new UserItemDataDto {UnplayedItemCount = 6}});
                RecentItems.Add(new BaseItemDto {Id = "2fc6f321b5f8bbe842fcd0eed089561d", Name = "A Night To Remember"});
            }
            else
            {
                WireCommands();
                DummyFolder = new BaseItemDto
                {
                    Type = "folder",
                    Name = AppResources.LabelRecent.ToLower()
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

                if (!_hasLoaded)
                {
                    ReviewReminderService.Current.Notify();
                }
            });

            RefreshDataCommand = new RelayCommand(async () =>
            {
                await GetEverything(true);
            });

            ChangeProfileCommand = new RelayCommand(() =>
            {
                Log.Info("Signing out");

                Reset();

                AuthenticationService.Current.SignOut().ConfigureAwait(false);

                NavigationService.NavigateTo(Constants.Pages.ChooseProfileView);
            });

            PinCollectionCommand = new RelayCommand<BaseItemDto>(collection =>
            {
                var tileUrl = string.Format(Constants.PhoneTileUrlFormat, "Collection", collection.Id, collection.Name);
                var existingTile = TileService.Current.GetTile(tileUrl);
                if (existingTile != default(ShellTileServiceTile))
                {
                    var result = MessageBox.Show(AppResources.MessageBoxUnpinText, AppResources.MessageAreYouSureTitle, MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        existingTile.Delete();
                        Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.Messages.CollectionPinnedMsg));
                    }
                    return;
                }

                TileService.Current.PinCollection(collection.Name, collection.Id, App.SpecificSettings.UseTransparentTile, true);
                Messenger.Default.Send(new NotificationMessage(tileUrl, Constants.Messages.CollectionPinnedMsg));

            });

            PlayMovieCommand = new RelayCommand<BaseItemDto>(async item =>
            {
                await PlayVideo(item);
            });

            ResumeMovieCommand = new RelayCommand<BaseItemDto>(async item =>
            {
                await PlayVideo(item, true);
            });

            NavigateToPage = new RelayCommand<BaseItemDto>(NavigationService.NavigateTo);

            NavigateToAPage = new RelayCommand<string>(NavigationService.NavigateTo);

            NavigateToNotificationsCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.NotifcationNavigationMsg));

                NavigationService.NavigateTo(Constants.Pages.NotificationsView);
            });
        }

        private async Task PlayVideo(BaseItemDto item, bool isResume = false)
        {
            if (item.IsAudio)
            {
                var playlistItem = await item.ToPlaylistItem(ApiClient, _playbackManager);
                Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(new List<PlaylistItem> { playlistItem }, Constants.Messages.SetPlaylistAsMsg));
                return;
            }

            Log.Info("Playing {0} [{1}]", item.Type, item.Name);
            if (!TrialHelper.Current.CanPlayVideo(item.Id))
            {
                TrialHelper.Current.ShowTrialMessage(AppResources.TrialVideoMessage);
            }
            else
            {
                if (SimpleIoc.Default.GetInstance<VideoPlayerViewModel>() != null && item.LocationType != LocationType.Virtual)
                {
                    if (item.UserData != null)
                    {
                        Messenger.Default.Send(new VideoMessage(item, isResume, item.UserData.PlaybackPositionTicks));
                    }
                    else
                    {
                        Messenger.Default.Send(new VideoMessage(item, isResume));
                    }

                    TrialHelper.Current.SetNewVideoItem(item.Id);
                    NavigationService.NavigateTo(string.Format(Constants.Pages.VideoPlayerView, item.Id, item.Type));
                }
            }

        }

        private void Reset()
        {
            AuthenticationService.Current.Logout();
            TileService.Current.ResetWideTile(App.SpecificSettings.UseTransparentTile);
            _hasLoaded = false;
            Folders.Clear();
            RecentItems.Clear();
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ResetAppMsg));
        }

        private async Task GetEverything(bool isRefresh)
        {
            if (NavigationService.IsNetworkAvailable
                && (!_hasLoaded || isRefresh))
            {

                SetProgressBar(AppResources.SysTrayLoadingCollections);

                var folderLoaded = await GetFolders();

                SetProgressBar(AppResources.SysTrayGettingRecentItems);

                var recentLoaded = await GetRecent();

                SetProgressBar(AppResources.SysTrayGettingFavourites);

                var favouritesLoaded = await GetFavouriteItems();

                SetProgressBar(AppResources.SysTrayCheckingInProgress);

                var inProgressLoaded = await GetInProgressItems();

                _hasLoaded = (folderLoaded && recentLoaded && favouritesLoaded && inProgressLoaded);

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
                UserId = AuthenticationService.Current.LoggedInUserId
            };
            var summary = await ApiClient.GetNotificationsSummary(AuthenticationService.Current.LoggedInUserId);
            var notifications = await ApiClient.GetNotificationsAsync(query);
        }

        private async Task<bool> GetInProgressItems()
        {
            try
            {
                Log.Info("Getting in progress items for user [{0}]", AuthenticationService.Current.LoggedInUserId);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Recursive = true,
                    Filters = new[] {ItemFilter.IsResumable,}
                };

                var items = await ApiClient.GetItemsAsync(query);
                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    InProgressItems.Clear();
                    items.Items.Foreach(InProgressItems.Add);
                }

                ShowInProgress = !InProgressItems.IsNullOrEmpty();
                Messenger.Default.Send(new NotificationMessage(ShowInProgress, Constants.Messages.ShowHideInProgressMsg));
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetInProgressItems()", ex, NavigationService, Log);
            }

            return false;
        }

        private async Task<bool> GetFavouriteItems()
        {
            try
            {
                FavouriteItems.Clear();
                Log.Info("Getting favourites for user [{0}]", AuthenticationService.Current.LoggedInUserId);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Filters = new[] {ItemFilter.IsFavorite},
                    Fields = new []{ ItemFields.MediaSources, ItemFields.SyncInfo },
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
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetFavouriteItems()", ex, NavigationService, Log);
                return false;
            }
        }

        private async Task<bool> GetRecent()
        {
            try
            {
                Log.Info("Getting most recent items");

                var query = Utils.GetRecentItemsQuery(excludedItemTypes: new[] { "Photo" });
                
                var items = await ApiClient.GetItemsAsync(query);
                _recentItems = items.Items;
                await SortRecent(_recentItems);
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetRecent()", ex, NavigationService, Log);
                return false;
            }
        }

        private async Task SortRecent(BaseItemDto[] items)
        {
            RecentItems.Clear();

            var recent = await Utils.SortRecentItems(items, App.SpecificSettings.IncludeTrailersInRecent);
            recent.Take(6).ToList().ForEach(recentItem => RecentItems.Add(recentItem));
        }

        private async Task<bool> GetFolders()
        {
            try
            {
                //Log.Info("Getting collections for [{0}] ({1})", AuthenticationService.Current.LoggedInUser.Name, AuthenticationService.Current.LoggedInUserId);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortOrder = SortOrder.Ascending,
                    SortBy = new[] {ItemSortBy.SortName}
                };

                var item = await ApiClient.GetItemsAsync(query);

                if (item != null && !item.Items.IsNullOrEmpty())
                {
                    Folders.Clear();

                    item.Items.OrderByDescending(x => x.SortName).ToList().ForEach(folder => Folders.Add(folder));

                    return true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetFolders()", ex, NavigationService, Log);
            }

            return false;
        }

        public RelayCommand MoreRecentCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DummyFolder.Name = AppResources.LabelRecent.ToLower();
                    NavigationService.NavigateTo(DummyFolder);
                });
            }
        }

        public RelayCommand MoreFavouritesCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DummyFolder.Name = AppResources.Favourites.ToLower();
                    NavigationService.NavigateTo(DummyFolder);
                });
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
        public RelayCommand<BaseItemDto> ResumeMovieCommand { get; set; }
        public ObservableCollection<BaseItemDto> Folders { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public ObservableCollection<BaseItemDto> FavouriteItems { get; set; }
        public ObservableCollection<BaseItemDto> InProgressItems { get; set; }
        public BaseItemDto DummyFolder { get; set; }
        public bool ShowInProgress { get; set; }
    }
}
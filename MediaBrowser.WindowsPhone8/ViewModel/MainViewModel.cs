using System;
using System.Collections.Generic;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Notifications;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Session;
using MediaBrowser.Services;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using MediaBrowser.Model.Dto;
using System.Threading.Tasks;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone.ViewModel;
using Microsoft.Phone.Tasks;
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
        private readonly IExtendedApiClient _apiClient;
        private bool _hasLoaded;
        private BaseItemDto[] _recentItems;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IExtendedApiClient apiClient, INavigationService navService)
        {
            _apiClient = apiClient;
            _navService = navService;

            Folders = new ObservableCollection<BaseItemDto>();
            RecentItems = new ObservableCollection<BaseItemDto>();
            FavouriteItems = new ObservableCollection<BaseItemDto>();

            if (IsInDesignMode)
            {
                Folders.Add(new BaseItemDto {Id = "78dbff5aa1c2101b98ebaf42b72a988d", Name = "Movies", RecursiveUnplayedItemCount = 6});
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

                _navService.NavigateTo(Constants.Pages.ChooseProfileView);
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

                //var tileDate = new ShellTileServiceCycleTileData
                //{
                //    Title = collection.Name,
                //    CycleImages = new Collection<Uri>
                //    {
                //        new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
                //        new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative)
                //    },
                //    SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative)
                //};

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

            NavigateToPage = new RelayCommand<BaseItemDto>(_navService.NavigateTo);

            NavigateToAPage = new RelayCommand<string>(_navService.NavigateTo);

            NavigateToNotificationsCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.NotifcationNavigationMsg));

                _navService.NavigateTo(Constants.Pages.NotificationsView);
            });
        }

        private async Task PlayVideo(BaseItemDto item, bool isResume = false)
        {
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
                    _navService.NavigateTo(string.Format(Constants.Pages.VideoPlayerView, item.Id, item.Type));
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
            if (_navService.IsNetworkAvailable
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
                UserId = AuthenticationService.Current.LoggedInUser.Id
            };
            var summary = await _apiClient.GetNotificationsSummary(AuthenticationService.Current.LoggedInUser.Id);
            var notifications = await _apiClient.GetNotificationsAsync(query);
        }

        private async Task<bool> GetFavouriteItems()
        {
            try
            {
                FavouriteItems.Clear();
                Log.Info("Getting favourites for user [{0}]", AuthenticationService.Current.LoggedInUser.Name);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
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
                Utils.HandleHttpException("GetFavouriteItems()", ex, _navService, Log);
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
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    Fields = new[]
                    {
                        ItemFields.DateCreated,
                        ItemFields.ParentId,
                    },
                    ExcludeItemTypes = new []{"Photo"},
                    IsVirtualUnaired = App.SpecificSettings.ShowUnairedEpisodes,
                    IsMissing = App.SpecificSettings.ShowMissingEpisodes,
                    Recursive = true
                };
                var items = await _apiClient.GetItemsAsync(query);
                _recentItems = items.Items;
                await SortRecent(_recentItems);
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetRecent()", ex, _navService, Log);
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
                Log.Info("Getting collections for [{0}] ({1})", AuthenticationService.Current.LoggedInUser.Name, AuthenticationService.Current.LoggedInUser.Id);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
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
                Utils.HandleHttpException("GetFolders()", ex, _navService, Log);
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
        public RelayCommand<BaseItemDto> ResumeMovieCommand { get; set; }
        public ObservableCollection<BaseItemDto> Folders { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public ObservableCollection<BaseItemDto> FavouriteItems { get; set; }
        public BaseItemDto DummyFolder { get; set; }
    }
}
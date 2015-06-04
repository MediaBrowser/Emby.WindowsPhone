using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using GalaSoft.MvvmLight.Messaging;
using Emby.WindowsPhone.Messaging;
using Emby.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;

namespace Emby.WindowsPhone.ViewModel.Predefined
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TvCollectionViewModel : ViewModelBase
    {
        private string _parentId;

        private bool _nextUpLoaded;
        private bool _latestUnwatchedLoaded;
        private bool _upcomingLoaded;
        private bool _showsLoaded;
        private bool _genresLoaded;

        private const int NextUpIndex = 0;
        private const int LatestIndex = 2;
        private const int ShowsIndex = 1;
        private const int UpcomingIndex = 4;
        private const int GenresIndex = 3;

        /// <summary>
        /// Initializes a new instance of the TvCollectionViewModel class.
        /// </summary>
        public TvCollectionViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            CreateCollections();
            if (IsInDesignMode)
            {
                NextUpList = new List<BaseItemDto>
                {
                    new BaseItemDto
                    {
                        Id = "e252ea3059d140a0274282bc8cd194cc",
                        Name = "1x01 - Pilot",
                        Overview =
                            "A Kindergarten teacher starts speaking gibberish and passed out in front of her class. What looks like a possible brain tumor does not respond to treatment and provides many more questions than answers for House and his team as they engage in a risky trial-and-error approach to her case. When the young teacher refuses any additional variations of treatment and her life starts slipping away, House must act against his code of conduct and make a personal visit to his patient to convince her to trust him one last time.",
                        SeriesName = "House M.D."
                    }
                };
            }
        }

        private void CreateCollections()
        {
            NextUpList = new List<BaseItemDto>();
            LatestUnwatched = new List<BaseItemDto>();
            Upcoming = new List<Group<BaseItemDto>>();
            Shows = new List<Group<BaseItemDto>>();
            Genres = new List<BaseItemDto>();
        }

        public List<BaseItemDto> NextUpList { get; set; }
        public List<BaseItemDto> LatestUnwatched { get; set; }
        public List<Group<BaseItemDto>> Upcoming { get; set; }
        public List<Group<BaseItemDto>> Shows { get; set; }
        public List<BaseItemDto> Genres { get; set; }

        public int PivotSelectedIndex { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetSelectedData(false);
                });
            }
        }

        public RelayCommand<BaseItemDto> NavigateToCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(item => NavigationService.NavigateTo(item));
            }
        }

        public RelayCommand<BaseItemDto> MarkAsWatchedCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    if (!NavigationService.IsNetworkAvailable)
                    {
                        return;
                    }

                    try
                    {
                        item.UserData = await ApiClient.MarkPlayedAsync(item.Id, AuthenticationService.Current.LoggedInUserId, DateTime.Now);
                    }
                    catch (HttpException ex)
                    {
                        MessageBox.Show(AppResources.ErrorProblemUpdatingItem, AppResources.ErrorTitle, MessageBoxButton.OK);
                        Utils.HandleHttpException("MarkAsWatchedCommand", ex, NavigationService, Log);
                    }
                });
            }
        }

        public RelayCommand<BaseItemDto> ItemOfflineCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    if (!item.CanTakeOffline()) return;

                    try
                    {
                        var request = SyncRequestHelper.CreateRequest(item.Id, item.GetTvTypeName());
                        await SyncService.Current.AddJobAsync(request);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("ItemOfflineCommand", ex, NavigationService, Log);
                    }
                });
            }
        }

        public void SetParentId(string parentId)
        {
            if (parentId != _parentId)
            {
                NextUpList.Clear();
                LatestUnwatched.Clear();
                Upcoming.Clear();
                Shows.Clear();
                Genres.Clear();

                _nextUpLoaded = _latestUnwatchedLoaded = _upcomingLoaded = _showsLoaded = _genresLoaded = false;

                _parentId = parentId;
            }
        }

        private async Task<bool> GetNextUp()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingNextUp);

                var query = new NextUpQuery
                {
                    ParentId = _parentId,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Fields = new[] { ItemFields.PrimaryImageAspectRatio, ItemFields.ParentId, ItemFields.MediaSources, ItemFields.SyncInfo },
                    ImageTypeLimit = 1,
                    EnableImageTypes = new []{ImageType.Backdrop, ImageType.Primary}
                };

                Log.Info("Getting next up items");

                var itemResponse = await ApiClient.GetNextUpEpisodesAsync(query);

                return SetNextUpItems(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetNextUp()", ex, NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> GetUpcoming()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingUpcoming);

                var query = new UpcomingEpisodesQuery
                {
                    ParentId = _parentId,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Fields = new[] { ItemFields.ParentId },
                    Limit = 30,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary}
                };

                Log.Info("Getting upcoming items");

                var itemResponse = await ApiClient.GetUpcomingEpisodesAsync(query);

                return SetUpcomingItems(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetUpcoming()", NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> GetLatestUnwatched()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingUnwatchedItems);

                var query = new ItemQuery
                {
                    ParentId = _parentId,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new[] { "DateCreated" },
                    SortOrder = SortOrder.Descending,
                    IncludeItemTypes = new[] { "Episode" },
                    Limit = 8,
                    Fields = new[] { ItemFields.PrimaryImageAspectRatio, ItemFields.ParentId, ItemFields.MediaSources, ItemFields.SyncInfo },
                    Filters = new[] { ItemFilter.IsUnplayed },
                    IsMissing = App.SpecificSettings.ShowMissingEpisodes,
                    IsUnaired = App.SpecificSettings.ShowUnairedEpisodes,
                    Recursive = true,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary}
                };

                Log.Info("Getting next up items");

                var itemResponse = await ApiClient.GetItemsAsync(query);

                return SetLatestUnwatched(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetLatestUnwatched()", ex, NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> GetShows()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingShows);

                var query = new ItemQuery
                {
                    ParentId = _parentId,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new[] { "SortName" },
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] { "Series" },
                    Fields = new[] { ItemFields.DateCreated, ItemFields.SyncInfo },
                    Recursive = true,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary}
                };

                Log.Info("Getting TV shows");

                var itemResponse = await ApiClient.GetItemsAsync(query);

                return await SetShows(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetShows()", ex, NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> GetGenres()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingGenres);

                var query = new ItemsByNameQuery
                {
                    ParentId = _parentId,
                    SortBy = new[] { "SortName" },
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] { "Series" },
                    Recursive = true,
                    Fields = new[] { ItemFields.DateCreated },
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary}
                };

                var items = await ApiClient.GetGenresAsync(query);

                if (!items.Items.IsNullOrEmpty())
                {
                    var genres = items.Items.ToList();
                    genres.ForEach(genre => genre.Type = "Genre - " + AppResources.LabelTv.ToUpper());

                    Genres = genres;

                    SetProgressBar();

                    return true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetGenres()", ex, NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> SetShows(ItemsResult itemResponse)
        {
            SetProgressBar();

            if (itemResponse == null || itemResponse.Items.IsNullOrEmpty())
            {
                return false;
            }

            Shows = await Utils.GroupItemsByName(itemResponse.Items.ToList());

            return true;
        }

        private bool SetLatestUnwatched(ItemsResult itemResponse)
        {
            SetProgressBar();

            if (itemResponse == null || !itemResponse.Items.Any())
            {
                return false;
            }

            LatestUnwatched = itemResponse.Items.ToList();

            return true;
        }

        private bool SetNextUpItems(ItemsResult itemResponse)
        {
            SetProgressBar();

            if (itemResponse == null || !itemResponse.Items.Any())
            {
                return false;
            }

            NextUpList = itemResponse.Items.ToList();

            return true;
        }

        private bool SetUpcomingItems(ItemsResult itemResponse)
        {
            SetProgressBar();

            if (itemResponse == null || !itemResponse.Items.Any())
            {
                return false;
            }

            var upcomingItems = itemResponse.Items;
            var groupedItems = (from u in upcomingItems
                                group u by u.PremiereDate.HasValue ? u.PremiereDate.Value.ToLocalTime().Date : DateTime.MinValue
                                    into grp
                                    orderby grp.Key
                                    select new Group<BaseItemDto>(Utils.CoolDateName(grp.Key.ToLocalTime().Date), grp)).ToList();

            Upcoming = groupedItems;

            return true;
        }

        [UsedImplicitly]
        private async void OnPivotSelectedIndexChanged()
        {
            await GetSelectedData(false);
        }

        private async Task GetSelectedData(bool isRefresh)
        {
            switch (PivotSelectedIndex)
            {
                case LatestIndex:
                    if (!NavigationService.IsNetworkAvailable || (_latestUnwatchedLoaded && !isRefresh))
                    {
                        return;
                    }

                    _latestUnwatchedLoaded = await GetLatestUnwatched();
                    break;
                case NextUpIndex:
                    if (!NavigationService.IsNetworkAvailable || (_nextUpLoaded && !isRefresh))
                    {
                        return;
                    }

                    _nextUpLoaded = await GetNextUp();
                    break;
                case UpcomingIndex:
                    if (!NavigationService.IsNetworkAvailable || (_upcomingLoaded && !isRefresh))
                    {
                        return;
                    }

                    _upcomingLoaded = await GetUpcoming();
                    break;
                case ShowsIndex:
                    if (!NavigationService.IsNetworkAvailable || (_showsLoaded && !isRefresh))
                    {
                        return;
                    }

                    _showsLoaded = await GetShows();
                    break;
                case GenresIndex:
                    if (!NavigationService.IsNetworkAvailable || (_genresLoaded && !isRefresh))
                    {
                        return;
                    }

                    _genresLoaded = await GetGenres();
                    break;
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<SyncNotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.SyncJobFinishedMsg))
                {
                    var type = m.ItemType.ToLower();
                    switch (type)
                    {
                        case "episode":
                            SetEpisodeAsWatched(NextUpList, m.ItemId);
                            SetEpisodeAsWatched(LatestUnwatched, m.ItemId);
                            break;
                        case "series":
                            foreach (var group in Shows)
                            {
                                var series = group.FirstOrDefault(x => x.Id == m.ItemId);
                                if (series != null)
                                {
                                    series.IsSynced = true;
                                    break;
                                }
                            }
                            break;
                    }
                }
            });
        }

        private static void SetEpisodeAsWatched(List<BaseItemDto> list, string itemId)
        {
            if (!list.IsNullOrEmpty())
            {
                var episode = list.FirstOrDefault(x => x.Id == itemId);
                if (episode != null)
                {
                    episode.IsSynced = true;
                }
            }
        }
    }
}
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
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Helpers;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;

namespace MediaBrowser.WindowsPhone.ViewModel.Predefined
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MovieCollectionViewModel : ViewModelBase
    {
        private bool _moviesLoaded;
        private bool _boxsetsLoaded;
        private bool _latestUnwatchedLoaded;
        private bool _genresLoaded;

        private const int LatestIndex = 0;
        private const int MoviesIndex = 1;
        private const int BoxsetsIndex = 2;
        private const int GenresIndex = 3;

        /// <summary>
        /// Initializes a new instance of the MovieCollectionViewModel class.
        /// </summary>
        public MovieCollectionViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            if (IsInDesignMode)
            {
                UnseenHeader = new BaseItemDto
                {
                    Id = "6536a66e10417d69105bae71d41a6e6f",
                    Name = "Jurassic Park",
                    SortName = "Jurassic Park",
                    Overview = "Lots of dinosaurs eating people!",
                    People = new[]
                    {
                        new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                        new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                        new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                        new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                    }
                };

                LatestUnwatched = new List<BaseItemDto>
                {
                    new BaseItemDto
                    {
                        Id = "6536a66e10417d69105bae71d41a6e6f",
                        Name = "Jurassic Park",
                        SortName = "Jurassic Park",
                        Overview = "Lots of dinosaurs eating people!",
                        People = new[]
                        {
                            new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                            new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                            new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                            new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                        }
                    },
                    new BaseItemDto
                    {
                        Id = "6536a66e10417d69105bae71d41a6e6f",
                        Name = "Jurassic Park",
                        SortName = "Jurassic Park",
                        Overview = "Lots of dinosaurs eating people!",
                        People = new[]
                        {
                            new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                            new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                            new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                            new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                        }
                    }
                };

                Movies = Utils.GroupItemsByName(LatestUnwatched).Result;
            }
        }

        public List<Group<BaseItemDto>> Movies { get; set; }
        public List<Group<BaseItemDto>> Boxsets { get; set; }
        public List<BaseItemDto> LatestUnwatched { get; set; }
        public List<BaseItemDto> Genres { get; set; }

        public BaseItemDto UnseenHeader { get; set; }

        public int PivotSelectedIndex { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetData(false);
                });
            }
        }

        public RelayCommand<BaseItemDto> NavigateToCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(NavigationService.NavigateTo);
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

                    var request = SyncRequestHelper.CreateRequest(item.Id, item.Name);
                    await SyncService.Current.AddJobAsync(request);
                });
            }
        }

        [UsedImplicitly]
        private async void OnPivotSelectedIndexChanged()
        {
            if (IsInDesignMode)
            {
                return;
            }

            await GetData(false);
        }

        private async Task GetData(bool isRefresh)
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
                case BoxsetsIndex:
                    if (!NavigationService.IsNetworkAvailable || (_boxsetsLoaded && !isRefresh))
                    {
                        return;
                    }

                    _boxsetsLoaded = await GetBoxsets();

                    break;
                case MoviesIndex:
                    if (!NavigationService.IsNetworkAvailable || (_moviesLoaded && !isRefresh))
                    {
                        return;
                    }

                    _moviesLoaded = await GetMovies();
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

        private async Task<bool> GetMovies()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingMovies);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new[] { "SortName" },
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] { "Movie" },
                    Recursive = true,
                    Fields = new[] { ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.SyncInfo },
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
                };

                var moviesResponse = await ApiClient.GetItemsAsync(query);

                return await SetMovies(moviesResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetMovies()", ex, NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> SetMovies(ItemsResult moviesResponse)
        {
            if (moviesResponse != null)
            {
                Movies = await Utils.GroupItemsByName(moviesResponse.Items);
            }

            SetProgressBar();

            return true;
        }

        private async Task<bool> GetBoxsets()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingBoxsets);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new[] { "SortName" },
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] { "BoxSet" },
                    Recursive = true,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, },
                    Fields = new[] { ItemFields.SyncInfo }
                };

                var itemResponse = await ApiClient.GetItemsAsync(query);
                return await SetBoxsets(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetBoxsets()", ex, NavigationService, Log);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> SetBoxsets(ItemsResult itemResponse)
        {
            if (itemResponse == null)
            {
                return false;
            }

            Boxsets = await Utils.GroupItemsByName(itemResponse.Items);
            SetProgressBar();

            return true;
        }

        private async Task<bool> GetLatestUnwatched()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingUnwatchedItems);

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new[] { "DateCreated" },
                    SortOrder = SortOrder.Descending,
                    IncludeItemTypes = new[] { "Movie" },
                    Limit = 9,
                    Fields = new[] { ItemFields.PrimaryImageAspectRatio },
                    Filters = new[] { ItemFilter.IsUnplayed },
                    Recursive = true,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
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

        private async Task<bool> GetGenres()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingGenres);

                var query = new ItemsByNameQuery
                {
                    SortBy = new[] { "SortName" },
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] { "Movie", "Trailer" },
                    Recursive = true,
                    Fields = new[] { ItemFields.DateCreated },
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
                };

                var items = await ApiClient.GetGenresAsync(query);

                if (!items.Items.IsNullOrEmpty())
                {
                    var genres = items.Items.ToList();
                    genres.ForEach(genre => genre.Type = "Genre - " + AppResources.LabelMovies.ToUpper());

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

        private bool SetLatestUnwatched(ItemsResult itemResponse)
        {
            if (itemResponse != null && !itemResponse.Items.IsNullOrEmpty())
            {
                var items = itemResponse.Items.ToList();
                UnseenHeader = items[0];

                items.RemoveAt(0);

                LatestUnwatched = items;

                SetProgressBar();

                return true;
            }
            return false;
        }
    }
}
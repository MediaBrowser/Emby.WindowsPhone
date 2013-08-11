using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;

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
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _genresLoaded;
        private bool _boxsetsLoaded;
        private bool _latestUnwatchedLoaded;

        /// <summary>
        /// Initializes a new instance of the MovieCollectionViewModel class.
        /// </summary>
        public MovieCollectionViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

            if (IsInDesignMode)
            {
                //Movies = new List<BaseItemDto>
                //{
                //    new BaseItemDto {Name = "Action", Type = "Genre"}
                //};

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
                return new RelayCommand<BaseItemDto>(_navigationService.NavigateTo);
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
                case 0:
                    if (!_navigationService.IsNetworkAvailable || (_latestUnwatchedLoaded && !isRefresh))
                    {
                        return;
                    }

                    _latestUnwatchedLoaded = await GetLatestUnwatched();

                    break;
                case 1:
                    if (!_navigationService.IsNetworkAvailable || (_boxsetsLoaded && !isRefresh))
                    {
                        return;
                    }

                    _boxsetsLoaded = await GetBoxsets();

                    break;
                case 2:
                    if (!_navigationService.IsNetworkAvailable || (_genresLoaded && !isRefresh))
                    {
                        return;
                    }

                    _genresLoaded = await GetMovies();
                    break;
            }
        }

        private async Task<bool> GetMovies()
        {
            try
            {
                SetProgressBar("Getting movies...");

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    SortBy = new[] { "SortName" },
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] { "Movie" },
                    Recursive = true,
                    Fields = new[] { ItemFields.ItemCounts, ItemFields.DateCreated }
                };

                var moviesResponse = await _apiClient.GetItemsAsync(query);

                if (moviesResponse != null)
                {
                    Movies = await Utils.GroupItemsByName(moviesResponse.Items);
                }

                SetProgressBar();

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetMovies()", ex);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> GetBoxsets()
        {
            try
            {
                SetProgressBar("Getting boxsets...");

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    SortBy = new[] {"SortName"},
                    SortOrder = SortOrder.Ascending,
                    IncludeItemTypes = new[] {"BoxSet"},
                    Recursive = true,
                    Fields = new[] {ItemFields.ItemCounts}
                };

                var itemResponse = await _apiClient.GetItemsAsync(query);
                if (itemResponse == null || !itemResponse.Items.Any())
                {
                    return false;
                }

                Boxsets = await Utils.GroupItemsByName(itemResponse.Items);
                SetProgressBar();

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetBoxsets()", ex);
            }

            SetProgressBar();

            return false;
        }

        private async Task<bool> GetLatestUnwatched()
        {
            try
            {
                SetProgressBar("Getting latest unwatched items...");

                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    SortBy = new[] {"DateCreated"},
                    SortOrder = SortOrder.Descending,
                    IncludeItemTypes = new[] {"Movie"},
                    Limit = 9,
                    Fields = new[] {ItemFields.PrimaryImageAspectRatio},
                    Filters = new[] {ItemFilter.IsUnplayed},
                    Recursive = true
                };

                Log.Info("Getting next up items");

                var itemResponse = await _apiClient.GetItemsAsync(query);

                if (itemResponse != null && itemResponse.Items.Any())
                {
                    var items = itemResponse.Items.ToList();
                    UnseenHeader = items[0];

                    items.RemoveAt(0);

                    LatestUnwatched = items;

                    SetProgressBar();

                    return true;
                }
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetLatestUnwatched()", ex);
            }

            SetProgressBar();

            return false;
        }
    }
}
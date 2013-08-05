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
    public class MusicCollectionViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _dataLoaded;
        private bool _artistsLoaded;
        private bool _albumsLoaded;
        private bool _songsLoaded;
        private bool _genresLoaded;

        /// <summary>
        /// Initializes a new instance of the MusicCollectionViewModel class.
        /// </summary>
        public MusicCollectionViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            if (IsInDesignMode)
            {
                var artists = new List<BaseItemDto> {new BaseItemDto {Name = "John Williams"}, new BaseItemDto {Name = "Hans Zimmer"}};
                Artists = Utils.GroupItemsByName(artists);
            }
        }

        public List<BaseItemDto> Genres { get; set; }
        public List<Group<BaseItemDto>> Songs { get; set; }
        public List<Group<BaseItemDto>> Artists { get; set; }
        public List<Group<BaseItemDto>> Albums { get; set; }
        public int PivotSelectedIndex { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetMusicCollection();
                });
            }
        }

        private async Task GetMusicCollection()
        {
            if (!_navigationService.IsNetworkAvailable || _dataLoaded)
            {
                return;
            }

            SetProgressBar("Getting artists...");

            await GetArtists();

            SetProgressBar();
        }

        [UsedImplicitly]
        private async void OnPivotSelectedIndexChanged()
        {
            switch (PivotSelectedIndex)
            {
                case 0: // Artists
                    if (!_navigationService.IsNetworkAvailable || _artistsLoaded)
                    {
                        return;
                    }

                    SetProgressBar("Getting artists...");
                    _artistsLoaded = await GetArtists();
                    SetProgressBar();
                    break;
                case 1: // Albums
                    if (!_navigationService.IsNetworkAvailable || _albumsLoaded)
                    {
                        return;
                    }

                    SetProgressBar("Getting albums...");
                    _albumsLoaded = await GetAlbums();
                    SetProgressBar();
                    break;
                case 2: // Songs
                    if (!_navigationService.IsNetworkAvailable || _songsLoaded)
                    {
                        return;
                    }

                    SetProgressBar("Getting songs...");
                    _songsLoaded = await GetSongs();
                    SetProgressBar();
                    break;
                case 3: // Genres
                    if (!_navigationService.IsNetworkAvailable || _genresLoaded)
                    {
                        return;
                    }

                    SetProgressBar("Getting genres...");
                    _genresLoaded = await GetGenres();
                    SetProgressBar();
                    break;
            }
        }

        private async Task<bool> GetArtists()
        {
            var query = new ArtistsQuery
            {
                SortBy = new []{"SortName"},
                SortOrder = SortOrder.Ascending,
                Recursive = true,
                UserId = AuthenticationService.Current.LoggedInUser.Id
            };

            try
            {
                var itemsResponse = await _apiClient.GetArtistsAsync(query);

                if (itemsResponse == null)
                {
                    return false;
                }

                var items = itemsResponse.Items.ToList();

                Artists = Utils.GroupItemsByName(items);

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetArtists()", ex);
            }

            return false;
        }

        private async Task<bool> GetGenres()
        {
            var query = new ItemsByNameQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                IncludeItemTypes = new[] { "Audio", "MusicVideo" },
                Recursive = true,
                Fields = new[] { ItemFields.ItemCounts, ItemFields.DateCreated },
                UserId = AuthenticationService.Current.LoggedInUser.Id
            };

            try
            {
                var genresResponse = await _apiClient.GetGenresAsync(query);

                if (genresResponse == null)
                {
                    return false;
                }

                Genres = genresResponse.Items.ToList();

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetGenres()", ex);
            }

            return false;
        }

        private async Task<bool> GetSongs()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Fields = new[] { ItemFields.ParentId, },
                IncludeItemTypes = new[] { "Audio" },
                UserId = AuthenticationService.Current.LoggedInUser.Id
            };

            try
            {
                var itemsResponse = await _apiClient.GetItemsAsync(query);

                if (itemsResponse == null)
                {
                    return false;
                }

                var items = itemsResponse.Items.ToList();

                Songs = Utils.GroupItemsByName(items);

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetSongs()", ex);
            }

            return false;
        }

        private async Task<bool> GetAlbums()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Fields = new[] { ItemFields.ParentId, },
                IncludeItemTypes = new[] { "MusicAlbum" },
                UserId = AuthenticationService.Current.LoggedInUser.Id
            };
            try
            {
                var itemsResponse = await _apiClient.GetItemsAsync(query);

                if (itemsResponse == null)
                {
                    return false;
                }

                var items = itemsResponse.Items.ToList();

                Albums = Utils.GroupItemsByName(items);

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetAlbums()", ex);
            }

            return false;
        }
    }
}
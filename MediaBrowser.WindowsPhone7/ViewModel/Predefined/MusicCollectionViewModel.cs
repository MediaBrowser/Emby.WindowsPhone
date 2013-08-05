using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
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

        /// <summary>
        /// Initializes a new instance of the MusicCollectionViewModel class.
        /// </summary>
        public MusicCollectionViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public List<BaseItemDto> Genres { get; set; }
        public List<Group<BaseItemDto>> Songs { get; set; }
        public List<Group<BaseItemDto>> Artists { get; set; }
        public List<Group<BaseItemDto>> Albums { get; set; }

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

            SetProgressBar("Loading music collection...");

            await GetArtists();

            await GetGenres();

            await GetSongs();

            await GetAlbums();
        }

        private async Task GetArtists()
        {
            var query = new ArtistsQuery
            {
                SortBy = new []{"SortName"},
                SortOrder = SortOrder.Ascending,
                Recursive = true
            };

            try
            {
                var itemsResponse = await _apiClient.GetArtistsAsync(query);

                if (itemsResponse == null)
                {
                    return;
                }

                var items = itemsResponse.Items.ToList();

                Artists = Utils.GroupItemsByName(items);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetArtists()", ex);
            }
        }

        private async Task GetGenres()
        {
            var query = new ItemsByNameQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                IncludeItemTypes = new[] { "Audio", "MusicVideo" },
                Recursive = true,
                Fields = new[] { ItemFields.ItemCounts, ItemFields.DateCreated }
            };

            try
            {
                var genresResponse = await _apiClient.GetGenresAsync(query);

                if (genresResponse != null)
                {
                    Genres = genresResponse.Items.ToList();
                }

                _dataLoaded = true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetGenres()", ex);
            }
        }

        private async Task GetSongs()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Fields = new[] { ItemFields.ParentId, },
                IncludeItemTypes = new[] { "Audio" }
            };

            try
            {
                var itemsResponse = await _apiClient.GetItemsAsync(query);

                if (itemsResponse == null)
                {
                    return;
                }

                var items = itemsResponse.Items.ToList();

                Songs = Utils.GroupItemsByName(items);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetSongs()", ex);
            }
        }

        private async Task GetAlbums()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Fields = new[] { ItemFields.ParentId, },
                IncludeItemTypes = new[] { "MusicAlbum" }
            };
            try
            {
                var itemsResponse = await _apiClient.GetItemsAsync(query);

                if (itemsResponse == null)
                {
                    return;
                }

                var items = itemsResponse.Items.ToList();

                Albums = Utils.GroupItemsByName(items);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetAlbums()", ex);
            }
        }
    }
}
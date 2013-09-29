using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using Microsoft.Phone.Shell;
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
            SelectedTracks = new List<BaseItemDto>();

            if (IsInDesignMode)
            {
                var artists = new List<BaseItemDto> {new BaseItemDto {Name = "John Williams"}, new BaseItemDto {Name = "Hans Zimmer"}};
                Artists = Utils.GroupItemsByName(artists).Result;
            }
        }

        public List<BaseItemDto> Genres { get; set; }
        public List<Group<BaseItemDto>> Songs { get; set; }
        public List<Group<BaseItemDto>> Artists { get; set; }
        public List<Group<BaseItemDto>> Albums { get; set; }
        public List<BaseItemDto> SelectedTracks { get; set; }
        public int PivotSelectedIndex { get; set; }
        public bool IsSelectionEnabled { get; set; }

        public ApplicationBarMode AppBarMode
        {
            get
            {
                return IsSelectionEnabled ? ApplicationBarMode.Minimized : ApplicationBarMode.Default;
            }
        }

        public Thickness SongsMargin
        {
            get
            {
                return IsSelectionEnabled ? new Thickness(0, 6, 0, 6) : new Thickness(-24, 6, 0, 6);
            }
        }

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

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand
        {
            get
            {
                return new RelayCommand<SelectionChangedEventArgs>(args =>
                {
                    if (args.AddedItems != null)
                    {
                        foreach (var track in args.AddedItems.Cast<BaseItemDto>())
                        {
                            SelectedTracks.Add(track);
                        }
                    }

                    if (args.RemovedItems != null)
                    {
                        foreach (var track in args.RemovedItems.Cast<BaseItemDto>())
                        {
                            SelectedTracks.Remove(track);
                        }
                    }

                    SelectedTracks = SelectedTracks.OrderBy(x => x.IndexNumber).ToList();
                });
            }
        }

        public RelayCommand SelectItemsCommand
        {
            get
            {
                return new RelayCommand(() => IsSelectionEnabled = true);
            }
        }

        public RelayCommand AddToNowPlayingCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var itemsResponse = new ItemsResult
                    {
                        Items = SelectedTracks.ToArray()
                    };

                    SendItemsToPlaylist(itemsResponse);
                });
            }
        }

        public RelayCommand<BaseItemDto> PlayItemCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    switch (item.Type.ToLower())
                    {
                        case "genre":
                            await GetGenreTracks(item.Name);
                            break;
                        case "musicalbum":
                            await GetAlbumTracks(item);
                            break;
                        case "artist":
                            await GetArtistTracks(item.Name);
                            break;
                    }
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
        
        private async Task GetAlbumTracks(BaseItemDto item)
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar("Getting album tracks...");

            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    Recursive = true,
                    Fields = new[] { ItemFields.ParentId, },
                    ParentId = item.Id,
                    IncludeItemTypes = new[] { "Audio" }
                };

                Log.Info("Getting tracks for album [{0}] ({1})", item.Name, item.Id);

                var itemResponse = await _apiClient.GetItemsAsync(query);

                SendItemsToPlaylist(itemResponse);
            }
            catch (HttpException ex)
            {
                Log.ErrorException(string.Format("GetAlbumTracks({0})", item.Name), ex);
            }

            SetProgressBar();
        }

        private async Task GetArtistTracks(string artistName)
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar("Getting artist tracks...");

            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    Artists = new[] {artistName},
                    Recursive = true,
                    Fields = new[] { ItemFields.ParentId,},
                    IncludeItemTypes = new[] {"Audio"}
                };

                Log.Info("Getting tracks for artist [{0}]", artistName);

                var itemResponse = await _apiClient.GetItemsAsync(query);

                SendItemsToPlaylist(itemResponse);
            }
            catch (HttpException ex)
            {
                Log.ErrorException(string.Format("GetArtistTracks({0})", artistName), ex);
            }

            SetProgressBar();
        }

        private async Task GetGenreTracks(string genreName)
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar("Getting genre tracks");

            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    Genres = new[] {genreName},
                    Recursive = true,
                    IncludeItemTypes = new[] {"Audio"}
                };

                Log.Info("Getting tracks for genre [{0}]", genreName);

                var itemResponse = await _apiClient.GetItemsAsync(query);

                SendItemsToPlaylist(itemResponse);
            }
            catch (HttpException ex)
            {
                Log.ErrorException(string.Format("GetGenreTracks({0})", genreName), ex);
            }

            SetProgressBar();
        }

        private void SendItemsToPlaylist(ItemsResult itemResponse)
        {
            if (itemResponse == null || itemResponse.Items.IsNullOrEmpty())
            {
                return;
            }

            var items = itemResponse.Items.ToList();

            var newList = items.ToPlayListItems(_apiClient);

            Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(newList, Constants.Messages.SetPlaylistAsMsg));

            _navigationService.NavigateTo(Constants.Pages.NowPlayingView);
        }

        private async Task GetMusicCollection()
        {
            if (!_navigationService.IsNetworkAvailable || _artistsLoaded)
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
            if (IsInDesignMode)
            {
                return;
            }

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

                Artists = await Utils.GroupItemsByName(items);

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
                Fields = new[] {  ItemFields.DateCreated },
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

                Songs = await Utils.GroupItemsByName(items);

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

                Albums = await Utils.GroupItemsByName(items);

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
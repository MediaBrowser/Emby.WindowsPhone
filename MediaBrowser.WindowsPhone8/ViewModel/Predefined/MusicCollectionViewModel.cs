using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using MediaBrowser.WindowsPhone.ViewModel.Playlists;
using Microsoft.Phone.Shell;
using ScottIsAFool.WindowsPhone;

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
        private bool _artistsLoaded;
        private bool _albumsLoaded;
        private bool _songsLoaded;
        private bool _genresLoaded;

        /// <summary>
        /// Initializes a new instance of the MusicCollectionViewModel class.
        /// </summary>
        public MusicCollectionViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            SelectedTracks = new List<BaseItemDto>();

            if (IsInDesignMode)
            {
                var artists = new List<BaseItemDto> {new BaseItemDto {Name = "John Williams"}, new BaseItemDto {Name = "Hans Zimmer"}};
                Artists = Utils.GroupItemsByName(artists).Result;
            }

            CanPlayAll = true;
        }

        public List<BaseItemDto> Genres { get; set; }
        public List<Group<BaseItemDto>> Songs { get; set; }
        public List<Group<BaseItemDto>> Artists { get; set; }
        public List<Group<BaseItemDto>> Albums { get; set; }
        public List<BaseItemDto> SelectedTracks { get; set; }
        public int PivotSelectedIndex { get; set; }
        public bool IsSelectionEnabled { get; set; }
        public bool CanPlayAll { get; set; }

        public ApplicationBarMode AppBarMode
        {
            get
            {
                return IsSelectionEnabled ? ApplicationBarMode.Minimized : ApplicationBarMode.Default;
            }
        }

        public int AppBarIndex
        {
            get { return PivotSelectedIndex == 2 ? 1 : 0; }
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

        public RelayCommand AddToPlaylistCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var vm = SimpleIoc.Default.GetInstance<AddToPlaylistViewModel>();
                    if (vm != null)
                    {
                        vm.AddMultipleToPlaylist.Execute(SelectedTracks);
                    }
                });
            }
        }

        public RelayCommand<BaseItemDto> StartInstantMixCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    SetProgressBar(AppResources.SysTrayGettingInstantMix);

                    try
                    {
                        var tracks = await ApiClient.GetInstantMixPlaylist(item);
                        Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(tracks, Constants.Messages.SetPlaylistAsMsg));
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "StartInstantMix", NavigationService, Log);
                    }

                    SetProgressBar();
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
                return new RelayCommand(async () =>
                {
                    var itemsResponse = new ItemsResult
                    {
                        Items = SelectedTracks.ToArray()
                    };

                    await SendItemsToPlaylist(itemsResponse);
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
                        case "audio":
                            var items = new ItemsResult
                            {
                                Items = new[]
                                {
                                    item
                                }
                            };

                            await SendItemsToPlaylist(items);
                            break;
                    }
                });
            }
        }

        public RelayCommand PlayAllCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    CanPlayAll = false;
                    SetProgressBar(AppResources.SysTrayGettingAllTracks);

                    if (Songs.IsNullOrEmpty())
                    {
                        _songsLoaded = await GetSongs();
                    }
                    
                    if(_songsLoaded)
                    {
                        var itemResult = new ItemsResult();
                        await Task.Factory.StartNew(async () =>
                        {
                            var tracks = Songs.SelectMany(x => x).ToArray();
                            itemResult = new ItemsResult
                            {
                                Items = tracks
                            };
                        });

                        await SendItemsToPlaylist(itemResult);
                    }

                    SetProgressBar();
                    CanPlayAll = true;
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
        
        private async Task GetAlbumTracks(BaseItemDto item)
        {
            if (!NavigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayGettingAlbumTracks);

            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Recursive = true,
                    Fields = new[] { ItemFields.ParentId, ItemFields.MediaSources },
                    ParentId = item.Id,
                    SortBy = new []{ItemSortBy.SortName},
                    IncludeItemTypes = new[] { "Audio" },
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
                };

                Log.Info("Getting tracks for album [{0}] ({1})", item.Name, item.Id);

                var itemResponse = await ApiClient.GetItemsAsync(query);

                await SendItemsToPlaylist(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(string.Format("GetAlbumTracks({0})", item.Name), ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetArtistTracks(string artistName)
        {
            if (!NavigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayGettingArtistTracks);

            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Artists = new[] {artistName},
                    Recursive = true,
                    Fields = new[] { ItemFields.ParentId, ItemFields.MediaSources},
                    IncludeItemTypes = new[] {"Audio"},
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
                };

                Log.Info("Getting tracks for artist [{0}]", artistName);

                var itemResponse = await ApiClient.GetItemsAsync(query);

                await SendItemsToPlaylist(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(string.Format("GetArtistTracks({0})", artistName), ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetGenreTracks(string genreName)
        {
            if (!NavigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayGettingGenreTracks);

            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Genres = new[] {genreName},
                    Recursive = true,
                    IncludeItemTypes = new[] {"Audio"}, 
                    Fields = new []{ ItemFields.MediaSources},
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
                };

                Log.Info("Getting tracks for genre [{0}]", genreName);

                var itemResponse = await ApiClient.GetItemsAsync(query);

                await SendItemsToPlaylist(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(string.Format("GetGenreTracks({0})", genreName), ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        private async Task SendItemsToPlaylist(ItemsResult itemResponse)
        {
            if (itemResponse == null || itemResponse.Items.IsNullOrEmpty())
            {
                return;
            }

            var items = itemResponse.Items.ToList();

            var newList = await items.ToPlayListItems(ApiClient);

            Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(newList, Constants.Messages.SetPlaylistAsMsg));

            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.NavigateTo(Constants.Pages.NowPlayingView));
        }

        private async Task GetMusicCollection()
        {
            if (!NavigationService.IsNetworkAvailable || _artistsLoaded)
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayGettingArtists);

            _artistsLoaded = await GetArtists();

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
                    if (!NavigationService.IsNetworkAvailable || _artistsLoaded)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayGettingArtists);
                    _artistsLoaded = await GetArtists();
                    SetProgressBar();
                    break;
                case 1: // Albums
                    if (!NavigationService.IsNetworkAvailable || _albumsLoaded)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayGettingAlbums);
                    _albumsLoaded = await GetAlbums();
                    SetProgressBar();
                    break;
                case 2: // Songs
                    if (!NavigationService.IsNetworkAvailable || _songsLoaded)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayGettingSongs);
                    _songsLoaded = await GetSongs();
                    SetProgressBar();
                    break;
                case 3: // Genres
                    if (!NavigationService.IsNetworkAvailable || _genresLoaded)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayGettingGenres);
                    _genresLoaded = await GetGenres();
                    SetProgressBar();
                    break;
            }
        }

        private async Task<bool> GetArtists()
        {
            var query = new ArtistsQuery
            {
                SortBy = new[] {"SortName"},
                Fields = new[] {ItemFields.SortName, ItemFields.MediaSources},
                SortOrder = SortOrder.Ascending,
                Recursive = true,
                UserId = AuthenticationService.Current.LoggedInUserId,
                ImageTypeLimit = 1,
                EnableImageTypes = new[] {ImageType.Backdrop, ImageType.Primary,}
            };

            try
            {
                var itemsResponse = await ApiClient.GetAlbumArtistsAsync(query);

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
                Utils.HandleHttpException("GetArtists()", ex, NavigationService, Log);
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
                Fields = new[] {  ItemFields.DateCreated, ItemFields.MediaSources },
                UserId = AuthenticationService.Current.LoggedInUserId,
                ImageTypeLimit = 1,
                EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
            };

            try
            {
                var genresResponse = await ApiClient.GetGenresAsync(query);

                if (genresResponse == null)
                {
                    return false;
                }

                Genres = genresResponse.Items.ToList();

                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetGenres()", ex, NavigationService, Log);
            }

            return false;
        }

        private async Task<bool> GetSongs()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Fields = new[] { ItemFields.ParentId, ItemFields.MediaSources },
                IncludeItemTypes = new[] { "Audio" },
                UserId = AuthenticationService.Current.LoggedInUserId,
                ImageTypeLimit = 1,
                EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
            };

            try
            {
                var itemsResponse = await ApiClient.GetItemsAsync(query);

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
                Utils.HandleHttpException("GetSongs()", ex, NavigationService, Log);
            }

            return false;
        }

        private async Task<bool> GetAlbums()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Fields = new[] { ItemFields.ParentId, ItemFields.SortName, ItemFields.MediaSources },
                IncludeItemTypes = new[] { "MusicAlbum" },
                UserId = AuthenticationService.Current.LoggedInUserId,
                ImageTypeLimit = 1,
                EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
            };
            try
            {
                var itemsResponse = await ApiClient.GetItemsAsync(query);

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
                Utils.HandleHttpException("GetAlbums()", ex, NavigationService, Log);
            }

            return false;
        }
    }
}
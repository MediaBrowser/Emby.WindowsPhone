using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MusicViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        private List<BaseItemDto> _artistTracks;
        private bool _gotAlbums;

        /// <summary>
        /// Initializes a new instance of the MusicViewModel class.
        /// </summary>
        public MusicViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            SelectedTracks = new List<BaseItemDto>();
            if (IsInDesignMode)
            {
                SelectedArtist = new BaseItemDto
                {
                    Name = "Hans Zimmer",
                    Id = "179d32421632781047c73c9bd501adea"
                };
                SelectedAlbum = new BaseItemDto
                {
                    Name = "The Dark Knight Rises",
                    Id = "f8d5c8cbcbd39bc75c2ba7ada65d4319",
                };
                Albums = new ObservableCollection<BaseItemDto>
                {
                    new BaseItemDto {Name = "The Dark Knight Rises", Id = "f8d5c8cbcbd39bc75c2ba7ada65d4319", ProductionYear = 2012},
                    new BaseItemDto {Name = "Batman Begins", Id = "03b6dbb15e4abcca6ee336a2edd79ba6", ProductionYear = 2005},
                    new BaseItemDto {Name = "Sherlock Holmes", Id = "6e2d519b958d440d034c3ba6eca008a4", ProductionYear = 2010}
                };
                AlbumTracks = new List<BaseItemDto>
                {
                    new BaseItemDto {Name = "Bombers Over Ibiza (Junkie XL Remix)", IndexNumber = 1, ParentIndexNumber = 2, RunTimeTicks = 3487920000, Id = "7589bfbe8b10d0191e305d92f127bd01"},
                    new BaseItemDto {Name = "A Storm Is Coming", Id = "1ea1fd991c70b33c596611dadf24defc", IndexNumber = 1, ParentIndexNumber = 1, RunTimeTicks = 369630000},
                    new BaseItemDto {Name = "On Thin Ice", Id = "2696da6a01f254fbd7e199a191bd5c4f", IndexNumber = 2, ParentIndexNumber = 1, RunTimeTicks = 1745500000},
                }.OrderBy(x => x.ParentIndexNumber)
                    .ThenBy(x => x.IndexNumber).ToList();

                SortedTracks = Utils.GroupArtistTracks(AlbumTracks);
            }
            else
            {
                WireCommands();
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.MusicArtistChangedMsg))
                {
                    Albums = new ObservableCollection<BaseItemDto>();
                    _artistTracks = new List<BaseItemDto>();
                    AlbumTracks = new List<BaseItemDto>();
                    SelectedArtist = (BaseItemDto) m.Sender;
                    _gotAlbums = false;
                }

                if (m.Notification.Equals(Constants.Messages.MusicAlbumChangedMsg))
                {
                    SelectedAlbum = (BaseItemDto) m.Sender;
                    if (_artistTracks != null)
                    {
                        AlbumTracks = _artistTracks.Where(x => x.ParentId == SelectedAlbum.Id)
                                                   .OrderBy(x => x.ParentIndexNumber)
                                                   .ThenBy(x => x.IndexNumber).ToList();
                    }
                }
            });
        }

        private void WireCommands()
        {
            ArtistPageLoaded = new RelayCommand(async () =>
            {
                if (_navigationService.IsNetworkAvailable && !_gotAlbums)
                {
                    SetProgressBar(AppResources.SysTrayGettingAlbums);

                    await GetArtistInfo();

                    SetProgressBar();
                }
            });

            AlbumPageLoaded = new RelayCommand(async () =>
            {
                if (AlbumTracks == null)
                {
                    SetProgressBar("Getting tracks...");
                   
                    try
                    {
                        await GetArtistInfo();

                        AlbumTracks = _artistTracks.Where(x => x.ParentId == SelectedAlbum.Id)
                            .OrderBy(x => x.ParentIndexNumber)
                            .ThenBy(x => x.IndexNumber).ToList();
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("AlbumPageLoaded", ex);
                    }
                }
            });

            AlbumTapped = new RelayCommand<BaseItemDto>(album =>
            {
                SelectedAlbum = album;
                AlbumTracks = _artistTracks.Where(x => x.ParentId == SelectedAlbum.Id)
                                           .OrderBy(x => x.IndexNumber)
                                           .ToList();
            });

            AlbumPlayTapped = new RelayCommand<BaseItemDto>(album =>
            {
                var albumTracks = _artistTracks.Where(x => x.ParentId == album.Id)
                                               .OrderBy(x => x.IndexNumber)
                                               .ToList();

                var newList = ConvertTracks(albumTracks);

                Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(newList, Constants.Messages.SetPlaylistAsMsg));
            });

            SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(args =>
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

            AddToNowPlayingCommand = new RelayCommand(() =>
            {
                if (!SelectedTracks.Any())
                {
                    return;
                }

                var newList = ConvertTracks(SelectedTracks);

                Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(newList, Constants.Messages.AddToPlaylistMsg));

                SelectedTracks = new List<BaseItemDto>();

                App.ShowMessage(string.Format("{0} tracks added successfully", newList.Count));

                IsInSelectionMode = false;
            });

            PlayItemsCommand = new RelayCommand(() =>
            {
                var newList = ConvertTracks(SelectedTracks);

                Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(newList, Constants.Messages.SetPlaylistAsMsg));
            });

            PlaySongCommand = new RelayCommand<BaseItemDto>(song =>
            {
                if (song == null)
                {
                    return;
                }

                var playlist = new List<PlaylistItem>
                {
                    song.ToPlaylistItem(_apiClient)
                };

                Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(playlist, Constants.Messages.SetPlaylistAsMsg));
            });
        }

        private async Task GetArtistInfo()
        {
            try
            {
                Log.Info("Getting information for Artist [{0}] ({1})", SelectedArtist.Name, SelectedArtist.Id);

                var artistQuery = new ArtistsQuery
                {

                };
                SelectedArtist = await _apiClient.GetItemAsync(SelectedArtist.Id, AuthenticationService.Current.LoggedInUser.Id);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetArtistInfo()", ex);
            }

            _gotAlbums = await GetAlbums();

            await GetArtistTracks();

            SortTracks();
        }

        private void SortTracks()
        {
            if (_artistTracks != null && _artistTracks.Any())
            {
                SortedTracks = Utils.GroupArtistTracks(_artistTracks);
            }
        }

        private List<PlaylistItem> ConvertTracks(List<BaseItemDto> list)
        {
            var newList = new List<PlaylistItem>();
            list.ForEach(item =>
            {
                var playlistItem = item.ToPlaylistItem(_apiClient);
                newList.Add(playlistItem);
            });

            return newList;
        }

        private async Task<bool> GetArtistTracks()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    Artists = new[] {SelectedArtist.Name},
                    Recursive = true,
                    Fields = new[] { ItemFields.ParentId,},
                    IncludeItemTypes = new[] {"Audio"}
                };

                Log.Info("Getting tracks for artist [{0}] ({1})", SelectedArtist.Name, SelectedArtist.Id);

                var items = await _apiClient.GetItemsAsync(query);

                if (items != null && items.Items.Any())
                {
                    _artistTracks = items.Items.ToList();
                }

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetArtistTracks()", ex);
                return false;
            }
        }

        private async Task<bool> GetAlbums()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    Artists = new[] {SelectedArtist.Name},
                    Recursive = true,
                    Fields = new[] { ItemFields.ParentId,},
                    IncludeItemTypes = new[] {"MusicAlbum"}
                };

                Log.Info("Getting albums for artist [{0}] ({1})", SelectedArtist.Name, SelectedArtist.Id);

                var items = await _apiClient.GetItemsAsync(query);
                if (items != null && items.Items.Any())
                {
                    //// Extract the album items from the results
                    //var albums = items.Items.Where(x => x.Type == "MusicAlbum").ToList();

                    //// Extract the track items from the results
                    //_artistTracks = items.Items.Where(y => y.Type == "Audio").ToList();

                    //var nameId = (from a in _artistTracks
                    //              select new KeyValuePair<string, string>(a.Album, a.ParentId)).Distinct();

                    //// This sets the album names correctly based on what's in the track information (rather than folder name)
                    //foreach (var ni in nameId)
                    //{
                    //    var item = albums.SingleOrDefault(x => x.Id == ni.Value);
                    //    item.Name = ni.Key;
                    //}

                    foreach (var album in items.Items)
                    {
                        Albums.Add(album);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.ErrorException("GetAlbums()", ex);
                return false;
            }
        }

        public bool IsInSelectionMode { get; set; }

        public int SelectedAppBarIndex
        {
            get { return IsInSelectionMode ? 1 : 0; }
        }

        public BaseItemDto SelectedArtist { get; set; }
        public BaseItemDto SelectedAlbum { get; set; }
        public ObservableCollection<BaseItemDto> Albums { get; set; }
        public List<BaseItemDto> AlbumTracks { get; set; }
        public List<Group<BaseItemDto>> SortedTracks { get; set; }
        public List<BaseItemDto> SelectedTracks { get; set; }

        public RelayCommand ArtistPageLoaded { get; set; }
        public RelayCommand AlbumPageLoaded { get; set; }
        public RelayCommand<BaseItemDto> AlbumTapped { get; set; }
        public RelayCommand<BaseItemDto> AlbumPlayTapped { get; set; }
        public RelayCommand<BaseItemDto> PlaySongCommand { get; set; }
        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }
        public RelayCommand AddToNowPlayingCommand { get; set; }
        public RelayCommand PlayItemsCommand { get; set; }
    }
}
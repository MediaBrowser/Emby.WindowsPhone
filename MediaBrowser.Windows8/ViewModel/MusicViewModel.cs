using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Windows8.Model;
using MetroLog;
using Windows.UI.Xaml;
using System.Linq;

namespace MediaBrowser.Windows8.ViewModel
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
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;

        private List<BaseItemDto> _artistTracks;
        private bool _gotAlbums;

        /// <summary>
        /// Initializes a new instance of the MusicViewModel class.
        /// </summary>
        public MusicViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

            Albums = new ObservableCollection<BaseItemDto>();
            _artistTracks = new List<BaseItemDto>();
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
                _logger = new DesignLogger();
            }
            else
            {
                WireMessages();
                WireCommands();
                _logger = LogManagerFactory.DefaultLogManager.GetLogger<MusicViewModel>();
            }
        }

        private void WireCommands()
        {
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.MusicArtistChangedMsg))
                {
                    Albums = new ObservableCollection<BaseItemDto>();
                    _artistTracks = new List<BaseItemDto>();
                    AlbumTracks = new List<BaseItemDto>();
                    SelectedArtist = (BaseItemDto)m.Sender;
                    _gotAlbums = false;
                }
                if (m.Notification.Equals(Constants.MusicAlbumChangedMsg))
                {
                    SelectedAlbum = (BaseItemDto)m.Sender;
                    AlbumTracks = _artistTracks.Where(x => x.ParentId == SelectedAlbum.Id)
                        .OrderBy(x => x.ParentIndexNumber)
                        .ThenBy(x => x.IndexNumber).ToList();
                }
                if (m.Notification.Equals(Constants.ArtistViewLoadedMsg))
                {
                    if (_navigationService.IsNetworkAvailable && !_gotAlbums)
                    {
                        ProgressText = "Getting albums...";
                        ProgressVisibility = Visibility.Visible;

                        _gotAlbums = await GetAlbums();

                        ProgressText = string.Empty;
                        ProgressVisibility = Visibility.Collapsed;
                    }
                }
                if (m.Notification.Equals(Constants.AlbumViewLoadedMsg))
                {

                }
            });
        }

        private async Task GetArtistInfo()
        {
            try
            {
                _logger.Info("Getting information for Artist [{0}] ({1})", SelectedArtist.Name, SelectedArtist.Id);

                SelectedArtist = await _apiClient.GetItemAsync(SelectedArtist.Id, App.Settings.LoggedInUser.Id);
            }
            catch (HttpException ex)
            {
                _logger.Fatal(ex.Message, ex);
            }

            _gotAlbums = await GetAlbums();

            SortTracks();
        }

        private void SortTracks()
        {
            if (_artistTracks != null && _artistTracks.Any())
            {
                SortedTracks = Utils.GroupArtistTracks(_artistTracks);
            }
        }

        private async Task<bool> GetAlbums()
        {
            try
            {
                var query = new ItemQuery
                                {
                                    UserId = App.Settings.LoggedInUser.Id,
                                    ParentId = SelectedArtist.Id,
                                    Recursive = true,
                                    Fields = new[] { ItemFields.AudioInfo, ItemFields.ParentId, }
                                };

                _logger.Info("Getting albums for artist [{0}] ({1})", SelectedArtist.Name, SelectedArtist.Id);
                var items = await _apiClient.GetItemsAsync(query);
                if (items != null && items.Items.Any())
                {
                    // Extract the album items from the results
                    var albums = items.Items.Where(x => x.Type == "MusicAlbum").ToList();

                    // Extract the track items from the results
                    _artistTracks = items.Items.Where(y => y.Type == "Audio").ToList();

                    var nameId = (from a in _artistTracks
                                  select new KeyValuePair<string, string>(a.Album, a.ParentId)).Distinct();

                    // This sets the album names correctly based on what's in the track information (rather than folder name)
                    foreach (var ni in nameId)
                    {
                        var item = albums.SingleOrDefault(x => x.Id == ni.Value);
                        item.Name = ni.Key;
                    }

                    foreach (var album in albums)
                    {
                        Albums.Add(album);
                    }
                    return true;
                }
                return false;
            }
            catch (HttpException ex)
            {
                _logger.Fatal(ex.Message, ex);
                return false;
            }
        }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }

        public BaseItemDto SelectedArtist { get; set; }
        public ObservableCollection<BaseItemDto> Albums { get; set; }
        public List<BaseItemDto> AlbumTracks { get; set; }
        public BaseItemDto SelectedAlbum { get; set; }
        public List<Group<BaseItemDto>> SortedTracks { get; set; }
    }
}
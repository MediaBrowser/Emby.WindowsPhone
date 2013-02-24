using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;

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
        private readonly ExtendedApiClient ApiClient;
        private readonly INavigationService NavigationService;

        private List<BaseItemDto> artistTracks;
        private bool gotAlbums;
        /// <summary>
        /// Initializes a new instance of the MusicViewModel class.
        /// </summary>
        public MusicViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            NavigationService = navigationService;
            ApiClient = apiClient;
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
            }
            else
            {
                WireCommands();
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.MusicArtistChangedMsg))
                {
                    Albums = new ObservableCollection<BaseItemDto>();
                    artistTracks = new List<BaseItemDto>();
                    AlbumTracks = new List<BaseItemDto>();
                    SelectedArtist = (BaseItemDto)m.Sender;
                    gotAlbums = false;
                }
                if (m.Notification.Equals(Constants.MusicAlbumChangedMsg))
                {
                    SelectedAlbum = (BaseItemDto)m.Sender;
                    AlbumTracks = artistTracks.Where(x => x.ParentId == SelectedAlbum.Id)
                        .OrderBy(x => x.ParentIndexNumber)
                        .ThenBy(x => x.IndexNumber).ToList();
                }
            });
        }

        private void WireCommands()
        {
            ArtistPageLoaded = new RelayCommand(async () =>
                                                    {
                                                        if (NavigationService.IsNetworkAvailable && !gotAlbums)
                                                        {
                                                            ProgressText = AppResources.SysTrayGettingAlbums;
                                                            ProgressIsVisible = true;

                                                            gotAlbums = await GetAlbums();

                                                            ProgressText = string.Empty;
                                                            ProgressIsVisible = false;
                                                        }
                                                    });

            AlbumPageLoaded = new RelayCommand(() =>
                                                   {
                                                       
                                                   });
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
                var items = await ApiClient.GetItemsAsync(query);
                if (items != null && items.Items.Any())
                {
                    var albums = items.Items.Where(x => x.Type == "MusicAlbum").ToList();
                    artistTracks = items.Items.Where(y => y.Type == "Audio").ToList();

                    var nameId = (from a in artistTracks
                                  select new KeyValuePair<string, string>(a.Album, a.ParentId)).Distinct();

                    foreach (var ni in nameId)
                    {
                        var item = albums.SingleOrDefault(x => x.Id == ni.Value);
                        item.Name = ni.Key;
                    }

                    foreach (var album in albums)
                    {
                        Albums.Add(album);
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public BaseItemDto SelectedArtist { get; set; }
        public BaseItemDto SelectedAlbum { get; set; }
        public ObservableCollection<BaseItemDto> Albums { get; set; }
        public List<BaseItemDto> AlbumTracks { get; set; }

        public RelayCommand ArtistPageLoaded { get; set; }
        public RelayCommand AlbumPageLoaded { get; set; }
    }
}
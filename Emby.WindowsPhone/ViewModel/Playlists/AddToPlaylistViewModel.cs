using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Playlists;
using MediaBrowser.Model.Querying;
using Emby.WindowsPhone.Converters;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;


namespace Emby.WindowsPhone.ViewModel.Playlists
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AddToPlaylistViewModel : ViewModelBase
    {
        private bool _playlistsLoaded;
        private List<BaseItemDto> _listOfItemsToAdd;

        /// <summary>
        /// Initializes a new instance of the AddToPlaylistViewModel class.
        /// </summary>
        public AddToPlaylistViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base (navigationService, connectionManager)
        {
        }

        public bool IsAddingToPlaylist { get; set; }
        public string PlaylistName { get; set; }
        public string AddingText { get; set; }

        public string AddingTo()
        {
            if (_listOfItemsToAdd.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (_listOfItemsToAdd.Count > 1)
            {
                var numberOfItems = string.Format(AppResources.LabelMultipleItems, _listOfItemsToAdd.Count);
                return string.Format(AppResources.LabelAddingXToPlaylist, numberOfItems);
            }

            var item = _listOfItemsToAdd[0];
            string text;
            switch (item.Type)
            {
                case "Season":
                    var season = string.Format(AppResources.LabelSeasonX, item.IndexNumber);
                    text = string.Format("{0} - {1}", item.SeriesName, season);
                    break;
                case "Episode":
                    var episodeName = new EpisodeNameConverter().Convert(item, typeof (string), null, null).ToString();
                    text = string.Format("{0} - {1}", item.SeriesName, episodeName);
                    break;
                case "Audio":
                    var artist = string.IsNullOrEmpty(item.AlbumArtist) ? string.Empty : string.Format("{0} - ", item.AlbumArtist);
                    text = string.Format("{0}{1}", artist, item.Name);
                    break;
                default:
                    text = item.Name;
                    break;
            }

            text = string.Format("'{0}'", text);
            return string.Format(AppResources.LabelAddingXToPlaylist, text);
        }

        public List<BaseItemDto> Playlists { get; set; }

        public bool ShowNewPlaylistName
        {
            get { return SelectedPlaylist != null && SelectedPlaylist.Name.Equals(AppResources.LabelNewPlaylist); }
        }

        public BaseItemDto SelectedPlaylist { get; set; }

        public RelayCommand<BaseItemDto> AddToPlaylistCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    if (item == null || !item.SupportsPlaylists)
                    {
                        return;
                    }

                    _listOfItemsToAdd = new List<BaseItemDto>{item};
                    
                    await RestOfAddToPlaylist();
                });
            }
        }

        private async Task RestOfAddToPlaylist()
        {
            AddingText = AddingTo();
            PlaylistName = string.Empty;
            NavigationService.NavigateTo(Constants.Pages.Playlists.AddToPlaylistView);

            await LoadPlaylists(false);

            if (!Playlists.IsNullOrEmpty())
            {
                SelectedPlaylist = Playlists.First();
            }
        }

        public RelayCommand<List<BaseItemDto>> AddMultipleToPlaylist
        {
            get
            {
                return new RelayCommand<List<BaseItemDto>>(async list =>
                {
                    if (list.IsNullOrEmpty())
                    {
                        return;
                    }

                    var items = list.Where(x => x.SupportsPlaylists).ToList();
                    _listOfItemsToAdd = items;

                    await RestOfAddToPlaylist();
                });
            }
        }

        public RelayCommand SaveToPlaylistCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (SelectedPlaylist == null)
                    {
                        return;
                    }

                    if (ShowNewPlaylistName)
                    {
                        if (string.IsNullOrEmpty(PlaylistName))
                        {
                            return;
                        }

                        await CreateNewPlaylist(_listOfItemsToAdd);
                    }
                    else
                    {
                        await AddToExistingPlaylist(_listOfItemsToAdd);
                    }
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadPlaylists(true);
                });
            }
        }

        private async Task CreateNewPlaylist(List<BaseItemDto> items)
        {
            var request = new PlaylistCreationRequest
            {
                UserId = AuthenticationService.Current.LoggedInUserId,
                MediaType = items.Any(x => x.IsAudio) ? "Audio" : "Video",
                ItemIdList = items.Select(x => x.Id).ToList(),
                Name = PlaylistName
            };

            try
            {
                SetProgressBar(AppResources.SysTrayAddingToPlaylist);

                var result = await ApiClient.CreatePlaylist(request);

                var playlist = new BaseItemDto
                {
                    Name = PlaylistName,
                    Id = result.Id,
                    MediaType = request.MediaType
                };

                Playlists.Add(playlist);

                PlaylistName = string.Empty;

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "CreateNewPlaylist()", NavigationService, Log);
                App.ShowMessage(AppResources.ErrorAddingToPlaylist);
            }

            SetProgressBar();
        }

        private async Task AddToExistingPlaylist(IEnumerable<BaseItemDto> items)
        {
            try
            {
                SetProgressBar(AppResources.SysTrayAddingToPlaylist);

                await ApiClient.AddToPlaylist(SelectedPlaylist.Id, items.Select(x => x.Id), AuthenticationService.Current.LoggedInUserId);
                PlaylistName = string.Empty;

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "AddToExistingPlaylist()", NavigationService, Log);
                App.ShowMessage(AppResources.ErrorAddingToPlaylist);
            }

            SetProgressBar();
        }

        private async Task LoadPlaylists(bool isRefresh)
        {
            if (!NavigationService.IsNetworkAvailable
                || (_playlistsLoaded && !isRefresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayLoadingPlaylists);
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    IncludeItemTypes = new[] { "Playlist" },
                    Recursive = true
                };

                var items = await ApiClient.GetItemsAsync(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    var playlists = items.Items.ToList();
                    playlists.Insert(0, new BaseItemDto { Name = AppResources.LabelNewPlaylist });

                    Playlists = playlists;
                    SelectedPlaylist = Playlists.First();
                    _playlistsLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadPlaylists(" + isRefresh + ")", NavigationService, Log);
                IsAddingToPlaylist = false;
            }

            SetProgressBar();
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.AddToServerPlaylistsMsg))
                {
                    await LoadPlaylists(false);

                    if (Playlists.Count > 0)
                    {
                        SelectedPlaylist = Playlists.First();
                    }
                }
            });
        }
    }
}
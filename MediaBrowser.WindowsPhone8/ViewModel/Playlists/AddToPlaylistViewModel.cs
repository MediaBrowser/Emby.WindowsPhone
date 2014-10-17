using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Playlists;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.Playlists
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AddToPlaylistViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _playlistsLoaded;
        private BaseItemDto _itemToAdd;

        /// <summary>
        /// Initializes a new instance of the AddToPlaylistViewModel class.
        /// </summary>
        public AddToPlaylistViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public bool IsAddingToPlaylist { get; set; }
        public string PlaylistName { get; set; }

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
                        if (_navigationService.CanGoBack)
                        {
                            _navigationService.GoBack();
                        }

                        return;
                    }

                    _itemToAdd = item;

                    _navigationService.NavigateTo(Constants.Pages.Playlists.AddToPlaylistView);

                    await LoadPlaylists(false);

                    if (!Playlists.IsNullOrEmpty())
                    {
                        SelectedPlaylist = Playlists.First();
                    }
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

                        await CreateNewPlaylist(_itemToAdd);
                    }
                    else
                    {
                        await AddToExistingPlaylist(_itemToAdd);
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

        private async Task CreateNewPlaylist(BaseItemDto item)
        {
            var request = new PlaylistCreationRequest
            {
                UserId = AuthenticationService.Current.LoggedInUserId,
                MediaType = item.IsAudio ? "Audio" : "Video",
                ItemIdList = new List<string> { item.Id },
                Name = PlaylistName
            };

            try
            {
                SetProgressBar(AppResources.SysTrayAddingToPlaylist);

                var result = await _apiClient.CreatePlaylist(request);

                var playlist = new BaseItemDto
                {
                    Name = PlaylistName,
                    Id = result.Id,
                    MediaType = request.MediaType
                };

                Playlists.Add(playlist);

                PlaylistName = string.Empty;

                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "CreateNewPlaylist()", _navigationService, Log);
                App.ShowMessage(AppResources.ErrorAddingToPlaylist);
            }

            SetProgressBar();
        }

        private async Task AddToExistingPlaylist(BaseItemDto item)
        {
            try
            {
                SetProgressBar(AppResources.SysTrayAddingToPlaylist);

                await _apiClient.AddToPlaylist(SelectedPlaylist.Id, new[] { item.Id }, AuthenticationService.Current.LoggedInUserId);
                PlaylistName = string.Empty;

                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "AddToExistingPlaylist()", _navigationService, Log);
                App.ShowMessage(AppResources.ErrorAddingToPlaylist);
            }

            SetProgressBar();
        }

        private async Task LoadPlaylists(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable
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

                var items = await _apiClient.GetItemsAsync(query);

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
                Utils.HandleHttpException(ex, "LoadPlaylists(" + isRefresh + ")", _navigationService, Log);
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
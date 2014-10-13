using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.Playlists
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ServerPlaylistsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _playlistLoaded;

        /// <summary>
        /// Initializes a new instance of the ServerPlaylistsViewModel class.
        /// </summary>
        public ServerPlaylistsViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            if (IsInDesignMode)
            {
                SelectedPlaylist = new BaseItemDto
                {
                    Name = "Jurassic Park"
                };
            }
        }

        public BaseItemDto SelectedPlaylist { get; set; }
        public List<BaseItemDto> PlaylistItems { get; set; }

        public bool IsAudioPlaylist
        {
            get { return SelectedPlaylist != null && SelectedPlaylist.Type.Equals("Playlist") && SelectedPlaylist.MediaType == "Audio"; }
        }

        public RelayCommand PlaylistViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (SelectedPlaylist == null ||
                !_navigationService.IsNetworkAvailable
                || (_playlistLoaded && !isRefresh))
            {
                return;
            }

            try
            {
                var query = new ItemQuery
                {
                    ParentId = SelectedPlaylist.Id,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };
                var items = await _apiClient.GetItemsAsync(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    PlaylistItems = items.Items.ToList();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", _navigationService, Log);
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ServerPlaylistChangedMsg))
                {
                    SelectedPlaylist = m.Sender as BaseItemDto;
                    _playlistLoaded = false;
                }
            });
        }
    }
}
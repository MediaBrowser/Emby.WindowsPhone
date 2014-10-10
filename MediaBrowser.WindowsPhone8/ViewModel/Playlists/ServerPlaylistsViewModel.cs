using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
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
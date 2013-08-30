using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Services;
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
    public class RemoteViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the RemoteViewModel class.
        /// </summary>
        public RemoteViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public bool IsLoading { get; set; }
        public bool SendingCommand { get; set; }
        public bool IsPinned { get; set; }
        public List<SessionInfoDto> Clients { get; set; }
        public SessionInfoDto SelectedClient { get; set; }

        public bool CanUseRemote
        {
            get
            {
                return Clients != null && Clients.Any() && SelectedClient != null;
            }
        }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    IsPinned = TileService.Current.TileExists(Constants.Pages.Remote.RemoteView);

                    await GetClients(false);
                });
            }
        }

        public RelayCommand PinTileCommand
        {
            get
            {
                return new RelayCommand(PinTile);
            }
        }

        public RelayCommand RefreshClientsCommand
        {
            get
            {
                return new RelayCommand(async () => await GetClients(true));
            }
        }

        public RelayCommand<string> SendPlaystateCommand
        {
            get
            {
                return new RelayCommand<string>(async commandString =>
                {
                    var request = new PlaystateRequest
                    {
                        Command = commandString.ToPlaystateCommandEnum()
                    };

                    try
                    {
                        SendingCommand = true;
                        await _apiClient.SendPlaystateCommandAsync(SelectedClient.Id.ToString(), request);
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("SendPlaystateCommand(" + commandString + ")", ex);
                    }

                    SendingCommand = false;
                });
            }
        }

        [UsedImplicitly]
        private async void OnSelectedClientChanged()
        {
            if (SelectedClient == null)
            {
                return;
            }

            await GetClientSession();
        }

        private async Task GetClientSession()
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            
        }

        private async Task GetClients(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_dataLoaded && !isRefresh))
            {
                return;
            }

            SetProgressBar("Getting clients...");

            try
            {
                var clients = await _apiClient.GetClientSessionsAsync();

                Clients = clients.Where(x => x.DeviceId != _apiClient.DeviceId && x.SupportsRemoteControl).ToList();

                if (SelectedClient != null)
                {
                    SelectedClient = Clients.FirstOrDefault(x => x.DeviceId == SelectedClient.DeviceId) ?? Clients[0];
                }
                else
                {
                    SelectedClient = Clients[0];
                }

                _dataLoaded = true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetClients()", ex);
            }

            SetProgressBar();
        }

        private void PinTile()
        {
            if (IsPinned)
            {
                // Unpin the tile
                var tile = TileService.Current.GetTile(Constants.Pages.Remote.RemoteView);
                tile.Delete();

                IsPinned = false;
            }
            else
            {
                var tileData = new ShellTileServiceFlipTileData
                {
                    Title = "MB Remote",
                    BackgroundImage = new Uri("/Assets/Tiles/MBRemoteTile.png", UriKind.Relative)
                };

                var tileUrl = string.Format(Constants.PhoneTileUrlFormat, "Remote", string.Empty, "Remote Control");
                TileService.Current.Create(new Uri(tileUrl, UriKind.Relative), tileData, false);

                IsPinned = true;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.ApiInteraction.WebSocket;
using MediaBrowser.Model;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone.ViewModel;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel.Remote
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

            if (IsInDesignMode)
            {
                Clients = new List<SessionInfoDto>
                {
                    new SessionInfoDto
                    {
                        Client = "Dashboard"
                    }
                };
            }
        }

        public bool IsLoading { get; set; }
        public bool SendingCommand { get; set; }
        public bool IsPinned { get; set; }
        public List<SessionInfoDto> Clients { get; set; }
        public SessionInfoDto SelectedClient { get; set; }
        public double PlayedPercentage { get; set; }
        public long? PlayedTicks { get; set; }
        public bool IsPlaying { get; set; }

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

                    await App.WebSocketClient.StartReceivingSessionUpdates(1500);

                    App.WebSocketClient.SessionsUpdated += WebSocketClientOnSessionsUpdated;

                    await GetClients(false);
                });
            }
        }

        public RelayCommand ClientPageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () => await GetClients(false));
            }
        }

        public RelayCommand PageUnloadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    App.WebSocketClient.SessionsUpdated -= WebSocketClientOnSessionsUpdated;

                    await App.WebSocketClient.StopReceivingSessionUpdates();
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
                    await SendCommand(commandString);
                });
            }
        }

        public RelayCommand PlayPauseCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var playPause = IsPlaying ? "Pause" : "Unpause";

                    await SendCommand(playPause);
                });
            }
        }

        public RelayCommand<int> SeekCommand
        {
            get
            {
                return new RelayCommand<int>(async seconds =>
                {
                    var ticks = TimeSpan.FromSeconds(seconds);

                    await SendCommand("Seek", ticks.Ticks);
                });
            }
        }

        private async Task SendCommand(string commandString, long? seekAmount = null)
        {
            var request = new PlaystateRequest
            {
                Command = commandString.ToPlaystateCommandEnum()
            };

            if (seekAmount.HasValue)
            {
                request.SeekPositionTicks = SelectedClient.NowPlayingPositionTicks + seekAmount.Value;
            }

            try
            {
                SendingCommand = true;
                await _apiClient.SendPlaystateCommandAsync(SelectedClient.Id, request);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("SendPlaystateCommand(" + commandString + ")", ex);
            }

            SendingCommand = false;
        }

        public RelayCommand<string> SendPlayCommand
        {
            get
            {
                return new RelayCommand<string>(async id =>
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        return;
                    }
                    
                    var clientId = "";
                    await _apiClient.SendPlayCommandAsync(clientId, new PlayRequest { ItemIds = new[] { id }, PlayCommand = PlayCommand.PlayNow });

                    _navigationService.NavigateTo(Constants.Pages.Remote.RemoteView);
                });
            }
        }

        public RelayCommand<BaseItemInfo> NavigateToCommand
        {
            get
            {
                return new RelayCommand<BaseItemInfo>(_navigationService.NavigateTo);
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

                if (!Clients.IsNullOrEmpty())
                {
                    if (SelectedClient != null)
                    {
                        SelectedClient = Clients.FirstOrDefault(x => x.DeviceId == SelectedClient.DeviceId) ?? Clients[0];
                    }
                    else
                    {
                        SelectedClient = Clients[0];
                    }

                    SetSessionDetails(SelectedClient);

                    _dataLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetClients()", ex);
            }

            SetProgressBar();
        }

        private void SetSessionDetails(SessionInfoDto selectedClient)
        {
            if (selectedClient.NowPlayingPositionTicks.HasValue &&  selectedClient.NowPlayingItem != null && selectedClient.NowPlayingItem.RunTimeTicks.HasValue)
            {
                PlayedTicks = selectedClient.NowPlayingPositionTicks;
                PlayedPercentage = ((double) selectedClient.NowPlayingPositionTicks/(double) selectedClient.NowPlayingItem.RunTimeTicks)*100;
            }

            IsPlaying = !selectedClient.IsPaused;
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

        private void WebSocketClientOnSessionsUpdated(object sender, SessionUpdatesEventArgs e)
        {
            if (SelectedClient == null)
            {
                return;
            }

            var session = e.Sessions.First(x => x.DeviceId == SelectedClient.DeviceId);


            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (SelectedClient.NowPlayingItem != null && session.NowPlayingItem != null && SelectedClient.NowPlayingItem.Id == session.NowPlayingItem.Id)
                {
                }
                else
                {
                    SelectedClient.NowPlayingItem = session.NowPlayingItem;
                }

                SetSessionDetails(session);
            });
        }
    }
}
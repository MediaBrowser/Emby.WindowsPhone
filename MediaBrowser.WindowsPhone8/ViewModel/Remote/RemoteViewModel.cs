using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Session;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Messaging;
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
        private string _videoId;
        private long? _startPositionTicks;

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
        public bool IsPaused { get; set; }
        public bool IsMuted { get; set; }
        public List<ChapterInfoDto> Chapters { get; set; }

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

                    ReviewReminderService.Current.Notify();

                    if (_startPositionTicks.HasValue)
                    {
                        SendCommand("Seek", _startPositionTicks.Value).ConfigureAwait(false);
                        _startPositionTicks = null;
                    }
                });
            }
        }

        public RelayCommand ShowChaptersCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
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
                    var playPause = IsPaused ? "Unpause" : "Pause";

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
                    var ticks = TimeSpan.FromSeconds(seconds).Ticks;

                    if (SelectedClient != null && SelectedClient.NowPlayingPositionTicks.HasValue)
                    {
                        ticks += SelectedClient.NowPlayingPositionTicks.Value;
                    }

                    await SendCommand("Seek", ticks);
                });
            }
        }

        public RelayCommand<SessionInfoDto> ClientSelectedCommand
        {
            get
            {
                return new RelayCommand<SessionInfoDto>(async client =>
                {
                    if (client == null)
                    {
                        return;
                    }

                    try
                    {
                        SelectedClient = client;

                        await _apiClient.SendPlayCommandAsync(SelectedClient.Id, new PlayRequest { ItemIds = new[] { _videoId }, PlayCommand = PlayCommand.PlayNow });

                        _navigationService.NavigateTo(Constants.Pages.Remote.RemoteView);
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("ClientSelectedCommand", ex);
                        MessageBox.Show("Unable to start your item.", "Error", MessageBoxButton.OK);
                        _navigationService.GoBack();
                    }
                });
            }
        }

        public RelayCommand SendMuteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (CanSendCommand())
                    {
                        try
                        {
                            await _apiClient.SendSystemCommandAsync(SelectedClient.Id, SystemCommand.ToggleMute);
                        }
                        catch (HttpException ex)
                        {
                            Log.ErrorException("SendMuteCommand", ex);
                        }
                    }
                });
            }
        }

        public RelayCommand<bool> AdjustVolumeCommand
        {
            get
            {
                return new RelayCommand<bool>(async isVolumeUp =>
                {
                    try
                    {
                        await _apiClient.SendSystemCommandAsync(SelectedClient.Id, isVolumeUp ? SystemCommand.VolumeUp : SystemCommand.VolumeDown);
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("AdjustVolumeCommand", ex);
                    }
                });
            }
        }

        private bool CanSendCommand()
        {
            if (TrialHelper.Current.CanRemoteControl(SelectedClient.Client))
            {
                return true;
            }

            TrialHelper.Current.ShowTrialMessage("In trial mode you can only control the web dashboard, to control more, please consider purchasing the full version.");
            return false;
        }

        private async Task SendCommand(string commandString, long? seekPosition = null)
        {
            if (CanSendCommand())
            {
                var request = new PlaystateRequest
                {
                    Command = commandString.ToPlaystateCommandEnum()
                };

                if (seekPosition.HasValue)
                {
                    request.SeekPositionTicks = seekPosition;
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
        }

        public RelayCommand<string> SendPlayCommand
        {
            get
            {
                return new RelayCommand<string>(id =>
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        return;
                    }

                    _videoId = id;
                    _startPositionTicks = null;
                    _navigationService.NavigateTo(Constants.Pages.Remote.ChooseClientView);
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
                var query = new SessionQuery
                {
                    ControllableByUserId = AuthenticationService.Current.LoggedInUserId,
                    SupportsRemoteControl = true
                };
                var clients = await _apiClient.GetClientSessionsAsync(query);

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

                    if (SelectedClient != null && SelectedClient.NowPlayingItem != null)
                    {
                        GetChapters(SelectedClient.NowPlayingItem).ConfigureAwait(false);
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
            if (selectedClient.NowPlayingPositionTicks.HasValue && selectedClient.NowPlayingItem != null && selectedClient.NowPlayingItem.RunTimeTicks.HasValue)
            {
                PlayedTicks = selectedClient.NowPlayingPositionTicks;
                PlayedPercentage = ((double)selectedClient.NowPlayingPositionTicks / (double)selectedClient.NowPlayingItem.RunTimeTicks) * 100;
            }

            IsPaused = selectedClient.IsPaused;
            IsMuted = selectedClient.IsMuted;
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

            //IsPaused = session.IsPaused;
            //IsMuted = session.IsMuted;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (SelectedClient.NowPlayingItem != null && session.NowPlayingItem != null && SelectedClient.NowPlayingItem.Id == session.NowPlayingItem.Id)
                {
                    SelectedClient.NowPlayingPositionTicks = session.NowPlayingPositionTicks;
                }
                else
                {
                    SelectedClient.NowPlayingItem = session.NowPlayingItem;
                    GetChapters(session.NowPlayingItem).ConfigureAwait(false);
                }

                SetSessionDetails(session);
            });
        }

        private async Task GetChapters(BaseItemInfo item)
        {
            try
            {
                var result = await _apiClient.GetItemAsync(item.Id, AuthenticationService.Current.LoggedInUserId);
                if (result != null)
                {
                    Chapters = result.Chapters;
                }
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetChapters()", ex);
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<RemoteMessage>(this, m =>
            {
                _videoId = m.ItemId;
                _startPositionTicks = m.StartPositionTicks;
            });
        }
    }
}
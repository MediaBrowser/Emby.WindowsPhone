using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Session;
using Emby.WindowsPhone.CimbalinoToolkit.Tiles;
using Emby.WindowsPhone.Messaging;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model.Dto;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.ViewModel.Remote
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RemoteViewModel : ViewModelBase
    {
        private bool _dataLoaded;
        private string _videoId;
        private long? _startPositionTicks;

        private readonly string _tileUrl = string.Format(Constants.PhoneTileUrlFormat, "Remote", string.Empty, "Remote Control");

        /// <summary>
        /// Initializes a new instance of the RemoteViewModel class.
        /// </summary>
        public RemoteViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
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
                    IsPinned = TileService.Current.TileExists(_tileUrl);

                    await StartWebSocket();
                });
            }
        }

        private async Task StartWebSocket()
        {
            await ApiClient.StartReceivingSessionUpdates(1500);

            ApiClient.SessionsUpdated += WebSocketClientOnSessionsUpdated;

            await GetClients(false);

            ReviewReminderService.Current.Notify();

            if (_startPositionTicks.HasValue)
            {
                SendCommand("Seek", _startPositionTicks.Value).ConfigureAwait(false);
                _startPositionTicks = null;
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
                return new RelayCommand(() =>
                {
                    StopWebSocket();
                });
            }
        }

        private async Task StopWebSocket()
        {
            ApiClient.SessionsUpdated -= WebSocketClientOnSessionsUpdated;

            await ApiClient.StopReceivingSessionUpdates().ConfigureAwait(false);
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

        public RelayCommand<string> SeekCommand
        {
            get
            {
                return new RelayCommand<string>(async secondsString =>
                {
                    var seconds = int.Parse(secondsString);
                    var ticks = TimeSpan.FromSeconds(seconds).Ticks;

                    if (SelectedClient != null && SelectedClient.PlayState.PositionTicks.HasValue)
                    {
                        ticks += SelectedClient.PlayState.PositionTicks.Value;
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
                        //ServerIdItem = SelectedClient;

                        await ApiClient.SendPlayCommandAsync(SelectedClient.Id, new PlayRequest { ItemIds = new[] { _videoId }, PlayCommand = PlayCommand.PlayNow });

                        NavigationService.NavigateTo(Constants.Pages.Remote.RemoteView);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("ClientSelectedCommand", ex, NavigationService, Log);
                        MessageBox.Show("Unable to start your item.", "Error", MessageBoxButton.OK);
                        NavigationService.GoBack();
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
                            var command = new GeneralCommand
                            {
                                Name = GeneralCommandType.ToggleMute.ToString()
                            };

                            await ApiClient.SendCommandAsync(SelectedClient.Id, command);
                        }
                        catch (HttpException ex)
                        {
                            Utils.HandleHttpException("SendMuteCommand", ex, NavigationService, Log);
                        }
                    }
                });
            }
        }

        public RelayCommand<string> AdjustVolumeCommand
        {
            get
            {
                return new RelayCommand<string>(async isVolumeUpString =>
                {
                    try
                    {
                        var isVolumeUp = bool.Parse(isVolumeUpString);
                        var command = new GeneralCommand
                        {
                            Name = isVolumeUp ? GeneralCommandType.VolumeUp.ToString() : GeneralCommandType.VolumeDown.ToString()
                        };

                        await ApiClient.SendCommandAsync(SelectedClient.Id, command);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("AdjustVolumeCommand", ex, NavigationService, Log);
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
                    await ApiClient.SendPlaystateCommandAsync(SelectedClient.Id, request);
                }
                catch (HttpException ex)
                {
                    Utils.HandleHttpException("SendPlaystateCommand(" + commandString + ")", ex, NavigationService, Log);
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
                    NavigationService.NavigateTo(Constants.Pages.Remote.ChooseClientView);
                });
            }
        }

        public RelayCommand<BaseItemDto> SendResumeCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    _videoId = item.Id;
                    _startPositionTicks = item.UserData.PlaybackPositionTicks;
                    NavigationService.NavigateTo(Constants.Pages.Remote.ChooseClientView);
                });
            }
        }

        public RelayCommand<BaseItemInfo> NavigateToCommand
        {
            get
            {
                return new RelayCommand<BaseItemInfo>(NavigationService.NavigateTo);
            }
        }

        private async Task GetClients(bool isRefresh)
        {
            if (!NavigationService.IsNetworkAvailable || (_dataLoaded && !isRefresh))
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayGettingClients);

            try
            {
                var query = new SessionQuery
                {
                    ControllableByUserId = AuthenticationService.Current.LoggedInUserId,
                };
                var clients = await ApiClient.GetClientSessionsAsync(query);

                Clients = clients.Where(x => x.DeviceId != ApiClient.DeviceId && x.SupportsRemoteControl).ToList();

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
                Utils.HandleHttpException("GetClients()", ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        private void SetSessionDetails(SessionInfoDto selectedClient)
        {
            if (selectedClient.PlayState.PositionTicks.HasValue)
            {
                PlayedTicks = selectedClient.PlayState.PositionTicks;
                if (selectedClient.NowPlayingItem != null && selectedClient.NowPlayingItem.RunTimeTicks.HasValue)
                {
                    PlayedPercentage = ((double)selectedClient.PlayState.PositionTicks / (double)selectedClient.NowPlayingItem.RunTimeTicks) * 100;
                }
            }

            IsPaused = selectedClient.PlayState.IsPaused;
            IsMuted = selectedClient.PlayState.IsMuted;
        }

        private void PinTile()
        {
            if (IsPinned)
            {
                // Unpin the tile
                var tile = TileService.Current.GetTile(_tileUrl);
                if (tile != null)
                {
                    tile.Delete();

                    IsPinned = false;
                }
            }
            else
            {
                var tileData = new ShellTileServiceFlipTileData
                {
                    Title = "Emby " + AppResources.LabelRemote,
                    BackgroundImage = App.SpecificSettings.UseTransparentTile ? new Uri("/Assets/Tiles/MBRemoteTileTransparent.png", UriKind.Relative) : new Uri("/Assets/Tiles/MBRemoteTile.png", UriKind.Relative)
                };

                TileService.Current.Create(new Uri(_tileUrl, UriKind.Relative), tileData, false);

                IsPinned = true;
            }
        }

        private void WebSocketClientOnSessionsUpdated(object sender, GenericEventArgs<SessionUpdatesEventArgs> args)
        {
            if (SelectedClient == null)
            {
                return;
            }

            var e = args.Argument;

            var session = e.Sessions.FirstOrDefault(x => x.DeviceId == SelectedClient.DeviceId);

            if (session == null)
            {
                return;
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (SelectedClient.NowPlayingItem != null && session.NowPlayingItem != null && SelectedClient.NowPlayingItem.Id == session.NowPlayingItem.Id)
                {
                    SelectedClient.PlayState.PositionTicks = session.PlayState.PositionTicks;
                }
                else
                {
                    SelectedClient.NowPlayingItem = session.NowPlayingItem;
                }

                SetSessionDetails(session);
            });
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<RemoteMessage>(this, m =>
            {
                _videoId = m.ItemId;
                _startPositionTicks = m.StartPositionTicks;
            });

            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ReconnectToWebSocketMsg))
                {
                    var client = ConnectionManager.GetApiClient(App.ServerInfo.Id);
                    client.OpenWebSocket(() => new WebSocketClient());

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    await StopWebSocket();
                    await StartWebSocket();
                }
            });
        }
    }
}
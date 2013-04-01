using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Shared;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using ScottIsAFool.WindowsPhone.IsolatedStorage;

#if WP8
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient ApiClient;
        private readonly INavigationService NavigationService;

        private const string PushServiceName = "MediaBrowser.WindowsPhone.PushService";

        public bool loadingFromSettings;
        /// <summary>
        /// Initializes a new instance of the PushViewModel class.
        /// </summary>
        public SettingsViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            if (IsInDesignMode)
            {
                IsRegistered = UseNotifications = true;
                RegisteredText = "Device registered";
                ServerPluginInstalled = false;

#if WP8
                FoundServers = new ObservableCollection<Server>
                                   {
                                       new Server {IpAddress = "192.168.0.2", PortNo = "8096"},
                                       new Server {IpAddress = "192.168.0.4", PortNo = "8096"}
                                   };
#endif
            }
            else
            {
                loadingFromSettings = true;
                SendTileUpdates = SendToastUpdates = true;
                RegisteredText = AppResources.DeviceNotRegistered;
                loadingFromSettings = false;
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.CheckForPushPluginMsg))
                                                                          {

                                                                          }
                                                                      });
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public string RegisteredText { get; set; }
        public bool SendToastUpdates { get; set; }
        public bool SendTileUpdates { get; set; }

        public bool IsRegistered { get; set; }
        public bool ServerPluginInstalled { get; set; }
        public bool UseNotifications { get; set; }
        public HttpNotificationChannel HttpNotificationChannel { get; set; }

        public RelayCommand SettingsPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                                            {
                                                if (NavigationService.IsNetworkAvailable && !string.IsNullOrEmpty(ApiClient.ServerHostName))
                                                {
                                                    try
                                                    {
                                                        //var result = await ApiClient.CheckForPushServer();
                                                        ServerPluginInstalled = true;
                                                    }
                                                    catch
                                                    {
                                                        ServerPluginInstalled = false;
                                                    }
                                                }
                                            });
            }
        }

        public RelayCommand<LiveTile> DeleteLiveTile
        {
            get
            {
                return new RelayCommand<LiveTile>(async liveTile =>
                                                      {

                                                      });
            }
        }

        public RelayCommand TestConnectionCommand
        {
            get
            {
                return new RelayCommand(async () => await TestConnection());
            }
        }

        private async Task TestConnection()
        {
            ProgressIsVisible = true;
            ProgressText = AppResources.SysTrayAuthenticating;
            if (NavigationService.IsNetworkAvailable)
            {
                if (await Utils.GetServerConfiguration(ApiClient))
                {
                    if (!IsInDesignMode)
                    {
                        ISettings.DeleteValue(Constants.SelectedUserSetting);
                        ISettings.SetKeyValue(Constants.ConnectionSettings, App.Settings.ConnectionDetails);
                    }
                    ProgressText = AppResources.SysTrayAuthenticating;
                    await Utils.CheckProfiles(NavigationService);
                }
                else
                {
                    App.ShowMessage("", AppResources.ErrorConnectionDetailsInvalid);
                }
            }
            ProgressIsVisible = false;
            ProgressText = string.Empty;
        }

#if WP8
        #region Server Broadcast code WP8 only
        public ObservableCollection<Server> FoundServers { get; set; }
        public RelayCommand FindServerLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                                            {
                                                if (!NavigationService.IsNetworkAvailable ||
                                                    (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet
                                                    && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211))
                                                    return;
                                                ProgressIsVisible = true;
                                                ProgressText = "Attempting to find your server...";

                                                await SendMessage("who is MediaBrowserServer?", 7359);

                                                ProgressText = string.Empty;
                                                ProgressIsVisible = false;
                                            });
            }
        }

        public RelayCommand<Server> ServerTappedCommand
        {
            get
            {
                return new RelayCommand<Server>(async server =>
                                                    {
                                                        App.Settings.ConnectionDetails.HostName = server.IpAddress;
                                                        App.Settings.ConnectionDetails.PortNo = int.Parse(server.PortNo);
                                                        //NavigationService.GoBack();

                                                        ProgressIsVisible = true;
                                                        ProgressText = AppResources.SysTrayAuthenticating;

                                                        await TestConnection();

                                                        ProgressIsVisible = false;
                                                        ProgressText = string.Empty;
                                                    });
            }
        }

        private async Task SendMessage(string message, int port)
        {
            FoundServers = new ObservableCollection<Server>();
            var socket = new DatagramSocket();

            socket.MessageReceived += SocketOnMessageReceived;

            using (var stream = await socket.GetOutputStreamAsync(new HostName("255.255.255.255"), port.ToString()))
            {
                using (var writer = new DataWriter(stream))
                {
                    var data = Encoding.UTF8.GetBytes(message);

                    writer.WriteBytes(data);
                    writer.StoreAsync();
                }
            }
        }

        private async void SocketOnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var result = args.GetDataStream();
            var resultStream = result.AsStreamForRead(1024);

            using (var reader = new StreamReader(resultStream))
            {
                var text = await reader.ReadToEndAsync();
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var parts = text.Split('|');

                    var fullAddress = parts[1].Split(':');

                    FoundServers.Add(new Server { IpAddress = fullAddress[0], PortNo = fullAddress[1] });
                });
            }
        }
        #endregion
#endif

        internal string DeviceId
        {
#if WP8
            get
            {
                var id = UserExtendedProperties.GetValue("ANID2") as string;
                return string.IsNullOrEmpty(id) ? "emulator" : id;
            }
#else
            get { return ParseANID(UserExtendedProperties.GetValue("ANID") as string); }
#endif
        }

        #region Push Notification methods
        private void OnServerPluginInstalledChanged()
        {
            if (!IsInDesignMode)
                ISettings.Set("ServerPluginInstalled", ServerPluginInstalled);
        }

        public async void OnUseNotificationsChanged()
        {
            if (!ServerPluginInstalled || loadingFromSettings) return;

            RegisterService();
        }

        public async Task RegisterService()
        {
            if (NavigationService.IsNetworkAvailable)
            {
                ProgressIsVisible = true;
                if (UseNotifications)
                {
                    ProgressText = AppResources.SysTrayRegisteringDevice;
                    HttpNotificationChannel = HttpNotificationChannel.Find(PushServiceName);

                    if (HttpNotificationChannel != null)
                    {
                        await SubscribeToService();
                    }
                    else
                    {
                        HttpNotificationChannel = new HttpNotificationChannel(PushServiceName);

                        HttpNotificationChannel.Open();
                    }
                }
                else
                {
                    ProgressText = AppResources.SysTrayUnregisteringDevice;

                    await ApiClient.DeleteDeviceAsync(DeviceId);

                    SendToastUpdates = SendTileUpdates = true;

                    if (HttpNotificationChannel.IsShellTileBound) HttpNotificationChannel.UnbindToShellTile();
                    if (HttpNotificationChannel.IsShellToastBound) HttpNotificationChannel.UnbindToShellToast();

                    IsRegistered = false;

                    ProgressText = string.Empty;
                    ProgressIsVisible = false;
                }
                if (!IsInDesignMode)
                    ISettings.Set("UseNotifications", UseNotifications);
            }
        }

        private void OnHttpNotificationChannelChanged()
        {
            SubscribeToChannelEvents();
        }

        private void OnIsRegisteredChanged()
        {
            RegisteredText = IsRegistered ? AppResources.DeviceRegistered : AppResources.DeviceNotRegistered;
            if (!IsInDesignMode) ISettings.Set("IsRegistered", IsRegistered);
        }

        private async void OnSendToastUpdatesChanged()
        {
            if (!loadingFromSettings)
            {
                try
                {
                    await ApiClient.UpdateDeviceAsync(DeviceId, SendToastUpdates);
                }
                catch
                {

                }
            }
            if (!IsInDesignMode) ISettings.Set("SendToastUpdates", SendToastUpdates);
        }

        private async void OnSendTileUpdatesChanged()
        {
            if (!loadingFromSettings)
            {
                try
                {
                    await ApiClient.UpdateDeviceAsync(DeviceId, sendTileUpdate: SendTileUpdates);
                }
                catch
                {

                }
            }
            if (!IsInDesignMode) ISettings.Set("SendTileUpdates", SendTileUpdates);
        }

        private async Task SubscribeToService()
        {
            var response = new RequestResult();
            try
            {
                await ApiClient.RegisterDeviceAsync(DeviceId, HttpNotificationChannel.ChannelUri.ToString(), SendTileUpdates, SendToastUpdates);
            }
            catch
            {
                var s = "";
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              IsRegistered = true;
                                                              BindingANotificationsChannelToAToastNotification();
                                                              ProgressText = string.Empty;
                                                              ProgressIsVisible = false;
                                                          });

        }

        private void SubscribeToChannelEvents()
        {
            if (HttpNotificationChannel == null) return;

            HttpNotificationChannel.ChannelUriUpdated += async (sender, args) => await SubscribeToService();

            HttpNotificationChannel.HttpNotificationReceived += (sender, args) =>
                                                                    {
                                                                        var s = "";
                                                                    };

            HttpNotificationChannel.ShellToastNotificationReceived += (sender, args) =>
                                                                          {
                                                                              if (args.Collection != null)
                                                                              {
                                                                                  var collection = (Dictionary<string, string>)args.Collection;

                                                                                  Deployment.Current.Dispatcher.BeginInvoke(() => App.ShowMessage("", collection["wp:Text2"]));

                                                                              }
                                                                          };

            HttpNotificationChannel.ErrorOccurred += (sender, args) =>
                                                         {
                                                             var s = "";
                                                         };
        }

        private void BindingANotificationsChannelToAToastNotification()
        {
            if (!HttpNotificationChannel.IsShellToastBound)
            {
                HttpNotificationChannel.BindToShellToast();
            }
            if (!HttpNotificationChannel.IsShellTileBound)
            {
                HttpNotificationChannel.BindToShellTile(new Collection<Uri> { new Uri("http://dev.scottisafool.co.uk") });
            }
        }
        #endregion

        private static string ParseANID(string anid)
        {
            if (!string.IsNullOrEmpty(anid))
            {
                var parts = anid.Split('&');
                var pairs = parts.Select(part => part.Split('='));
                var id = pairs
                    .Where(pair => pair.Length == 2 && pair[0] == "A")
                    .Select(pair => pair[1])
                    .FirstOrDefault();
                return id;
            }
            return "emulator";
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MediaBrowser.Windows8.Model;
using ReflectionIT.Windows8.Helpers;
using WinRtUtility;
using Windows.Networking;
using Windows.Networking.PushNotifications;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MediaBrowser.Windows8.ViewModel
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
        private readonly NavigationService NavigationService;
        public bool loadingFromSettings;
        /// <summary>
        /// Initializes a new instance of the NotificationsViewModel class.
        /// </summary>
        public SettingsViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            RegisteredText = "Device not registered";
            if (IsInDesignMode)
            {
                UseNotifications = ServerPluginInstalled = true;
                FoundServers = new ObservableCollection<Server>
                                   {
                                       new Server {IpAddress = "192.168.0.2", PortNo = "8096"},
                                       new Server {IpAddress = "192.168.0.4", PortNo = "8096"}
                                   };
            }
            else
            {
                WireMessages();
                loadingFromSettings = true;
                SendTileUpdates = SendToastUpdates = true;
                loadingFromSettings = false;
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.NotificationSettingsLoadedMsg))
                                                                          {
                                                                              if (!ServerPluginInstalled)
                                                                              {
                                                                                  await CheckForServerPlugin();
                                                                              }
                                                                          }

                                                                          if (m.Notification.Equals(Constants.CheckForPushPluginMsg))
                                                                          {
                                                                              await CheckForServerPlugin();
                                                                          }
                                                                      });
        }

        private async Task CheckForServerPlugin()
        {
            try
            {
                ProgressText = "Checking server for plugin...";
                ProgressVisibility = Visibility.Visible;

                //var result = await ApiClient.CheckForPushServer();
                ServerPluginInstalled = true;

                await SavePushSettings();

                ProgressText = string.Empty;
                ProgressVisibility = Visibility.Collapsed;
            }
            catch { }
        }

        public string DeviceId
        {
            get { return ApiClient.DeviceName + Windows.System.UserProfile.UserInformation.GetDisplayNameAsync().GetResults(); }
        }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }

        #region Find Server

        public RelayCommand FindServersCommand
        {
            get
            {
                return new RelayCommand(async () =>
                                                  {
                                                      await SendMessage("who is MediaBrowserServer?", 7359);
                                                  });
            }
        }

        public RelayCommand<Server> ServerTappedCommand
        {
            get
            {
                return new RelayCommand<Server>(server =>
                                                    {
                                                        App.Settings.ConnectionDetails.HostName = server.IpAddress;
                                                        App.Settings.ConnectionDetails.PortNo = int.Parse(server.PortNo);
                                                        SimpleIoc.Default.GetInstance<LoadingViewModel>().TestConnectionCommand.Execute(null);
                                                    });
            }
        }

        public ObservableCollection<Server> FoundServers { get; set; }

        private async Task SendMessage(string message, int port)
        {
            var socket = new DatagramSocket();
            FoundServers = new ObservableCollection<Server>();

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

                var parts = text.Split('|');

                var fullAddress = parts[1].Split(':');

                App.ThisWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => FoundServers.Add(new Server { IpAddress = fullAddress[0], PortNo = fullAddress[1] }));
            }
        }
        #endregion

        #region Push Notifications
        public string RegisteredText { get; set; }
        public bool SendToastUpdates { get; set; }
        public bool SendTileUpdates { get; set; }

        public bool IsRegistered { get; set; }
        public bool ServerPluginInstalled { get; set; }
        public bool UseNotifications { get; set; }
        public PushNotificationChannel PushNotificationChannel { get; set; }

        private async void OnIsRegisteredChanged()
        {
            RegisteredText = IsRegistered ? "Device registered" : "Device not registered";
            await SavePushSettings();
        }

        private async void OnUseNotificationsChanged()
        {
            if (!ServerPluginInstalled || loadingFromSettings) return;
            if (NavigationService.IsNetworkAvailable)
            {
                if (UseNotifications)
                {
                    await RegisterDevice();
                }
                else
                {
                    await DeleteDevice();
                }
                await SavePushSettings();
            }
        }

        private async Task DeleteDevice()
        {
            try
            {
                await ApiClient.DeleteDeviceAsync(DeviceId);
                IsRegistered = true;
            }
            catch
            {

            }
        }

        private async Task RegisterDevice()
        {
            try
            {
                PushNotificationChannel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                PushNotificationChannel.PushNotificationReceived += (sender, args) =>
                                                                        {

                                                                        };
                var url = PushNotificationChannel.Uri.Replace("%", "IMAPERCENT");
                await ApiClient.RegisterDeviceAsync(Uri.EscapeDataString(DeviceId), url, SendTileUpdates, SendToastUpdates);
                IsRegistered = true;
            }
            catch
            {

            }
        }

        private async void OnSendToastUpdatesChanged()
        {
            if (loadingFromSettings) return;
            try
            {
                await ApiClient.UpdateDeviceAsync(DeviceId, SendToastUpdates);
                await SavePushSettings();
            }
            catch
            {

            }
        }

        private async Task SavePushSettings()
        {
            var storageHelper = new ObjectStorageHelper<bool>(StorageType.Local);

            await storageHelper.SaveAsync(ServerPluginInstalled, "ServerPluginInstalled");
            await storageHelper.SaveAsync(UseNotifications, "UseNotifications");
            await storageHelper.SaveAsync(SendTileUpdates, "SendTileUpdates");
            await storageHelper.SaveAsync(SendToastUpdates, "SendToastUpdates");
            await storageHelper.SaveAsync(IsRegistered, "IsRegistered");
        }

        private async void OnSendTileUpdatesChanged()
        {
            if (loadingFromSettings) return;
            try
            {
                await ApiClient.UpdateDeviceAsync(DeviceId, sendTileUpdate: SendTileUpdates);
                await SavePushSettings();
            }
            catch
            {

            }
        }
        #endregion
    }
}
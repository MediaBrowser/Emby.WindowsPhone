using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using MediaBrowser.WindowsPhone.Model;
using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;
using ScottIsAFool.WindowsPhone.IsolatedStorage;

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
            }
            else
            {
                loadingFromSettings = true;
                SendTileUpdates = SendToastUpdates = true;
                RegisteredText = "Device not registered";
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
                                                if (NavigationService.IsNetworkAvailable)
                                                {
                                                    try
                                                    {
                                                        var result = await ApiClient.CheckForPushServer();
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

        private void OnServerPluginInstalledChanged()
        {
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
                    ProgressText = "Registering device...";
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
                    ProgressText = "Unregistering device...";

                    var response = await ApiClient.DeleteDevice(DeviceId);
                    
                    SendToastUpdates = SendTileUpdates = true;
                    
                    if(HttpNotificationChannel.IsShellTileBound) HttpNotificationChannel.UnbindToShellTile();
                    if(HttpNotificationChannel.IsShellToastBound) HttpNotificationChannel.UnbindToShellToast();

                    IsRegistered = false;

                    ProgressText = string.Empty;
                    ProgressIsVisible = false;
                }
                ISettings.Set("UseNotifications", UseNotifications);
            }
        }

        private void OnHttpNotificationChannelChanged()
        {
            SubscribeToChannelEvents();
        }

        private void OnIsRegisteredChanged()
        {
            RegisteredText = IsRegistered ? "Device registered" : "Device not registered";
            ISettings.Set("IsRegistered", IsRegistered);
        }

        private async void OnSendToastUpdatesChanged()
        {
            if (!loadingFromSettings)
            {
                try
                {
                    await ApiClient.UpdateDevice(DeviceId, SendToastUpdates);
                }
                catch
                {

                }
            }
            ISettings.Set("SendToastUpdates", SendToastUpdates);
        }

        private async void OnSendTileUpdatesChanged()
        {
            if (!loadingFromSettings)
            {
                try
                {
                    await ApiClient.UpdateDevice(DeviceId, liveTile: SendTileUpdates);
                }
                catch
                {

                }
            }
            ISettings.Set("SendTileUpdates", SendTileUpdates);
        }

        private async Task SubscribeToService()
        {
            var response = new RequestResult();
            try
            {
                response = await ApiClient.RegisterDevice(DeviceId, HttpNotificationChannel.ChannelUri.ToString(), SendTileUpdates, SendToastUpdates);
            }
            catch
            {
                var s = "";
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              IsRegistered = response.Success;
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
                HttpNotificationChannel.BindToShellTile(new Collection<Uri>{ new Uri("http://dev.scottisafool.co.uk")});
            }
        }

        private static string ParseANID(string anid)
        {
            if (!string.IsNullOrEmpty(anid))
            {
                string[] parts = anid.Split('&');
                IEnumerable<string[]> pairs = parts.Select(part => part.Split('='));
                var id = pairs
                    .Where(pair => pair.Length == 2 && pair[0] == "A")
                    .Select(pair => pair[1])
                    .FirstOrDefault();
                return id;
            }
            else
            {
                return "emulator";
            }

            return string.Empty;
        }
    }
}
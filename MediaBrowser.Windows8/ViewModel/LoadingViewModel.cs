using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MediaBrowser.Windows8.Views;
using MetroLog;
using ReflectionIT.Windows8.Helpers;
using WinRtUtility;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.ApplicationSettings;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using MediaBrowser.Model;
using MediaBrowser.Shared;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LoadingViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly NavigationService _navService;
        private readonly ILogger _logger;

        private bool _checksDone;
        private bool _isFromSearch;
        private string _searchTerm;

        /// <summary>
        /// Initializes a new instance of the LoadingViewModel class.
        /// </summary>
        public LoadingViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            _apiClient = apiClient;
            _navService = navigationService;
            if (IsInDesignMode)
            {
                _logger = new DesignLogger();
            }
            else
            {
                WireMessages();
                WireCommands();
                _logger = LogManagerFactory.DefaultLogManager.GetLogger<LoadingViewModel>();
            }
        }

        private void WireCommands()
        {
            TestConnectionCommand = new RelayCommand(async () =>
            {
                Messenger.Default.Send(new NotificationMessage(Constants.ClearEverythingMsg));

                _apiClient.ServerHostName = App.Settings.ConnectionDetails.HostName;
                _apiClient.ServerApiPort = App.Settings.ConnectionDetails.PortNo;
                var isNotLocalhost = CheckForLocalhost();
#if !DEBUG
                if (!isNotLocalhost)
                {
                    await MessageBox.ShowAsync("Unfortunately, this app can't be run on the same machine as the server, this is a limitation of the platform.", "Please use a different hostname", MessageBoxButton.OK);
                    return;
                }
#endif
                var userSettings = new ObjectStorageHelper<UserSettingWrapper>(StorageType.Roaming);
                await userSettings.DeleteAsync(Constants.SelectedUserSetting);

                var connectionSuccess = await CheckForServer();
                if (connectionSuccess)
                {
                    var connectionSettings = new ObjectStorageHelper<ConnectionDetails>(StorageType.Roaming);
                    
                    await connectionSettings.SaveAsync(new ConnectionDetails
                    {
                        HostName = App.Settings.ConnectionDetails.HostName,
                        PortNo = App.Settings.ConnectionDetails.PortNo
                    }, Constants.ConnectionSettings);

                }
            });

            ClearConnectionSettingsCommand = new RelayCommand(async () =>
                                                                  {
                                                                      var connectionSettings = new ObjectStorageHelper<ConnectionDetails>(StorageType.Roaming);
                                                                      await connectionSettings.DeleteAsync(Constants.ConnectionSettings);
                                                                      App.Settings.ConnectionDetails = new ConnectionDetails {PortNo = 8096};
                                                                  });
        }

        private bool CheckForLocalhost()
        {
            var profiles = NetworkInformation.GetConnectionProfiles().ToList();

            // the Internet connection profile doesn't seem to be in the above list
            profiles.Add(NetworkInformation.GetInternetConnectionProfile());

            IEnumerable<HostName> hostnames =
                NetworkInformation.GetHostNames().Where(h =>
                    h.IPInformation != null &&
                    h.IPInformation.NetworkAdapter != null).ToList();

            var ipAddresses = (from h in hostnames
                    from p in profiles
                    where h.IPInformation.NetworkAdapter.NetworkAdapterId ==
                          p.NetworkAdapter.NetworkAdapterId
                    select h.CanonicalName).ToList();

            var machineIpAddress = ipAddresses[0];

            return App.Settings.ConnectionDetails.HostName != "127.0.0.1" &&
                App.Settings.ConnectionDetails.HostName.ToLower() != "localhost" &&
                App.Settings.ConnectionDetails.HostName != machineIpAddress &&
                App.Settings.ConnectionDetails.HostName.ToLower() != _apiClient.DeviceName.ToLower();
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m=>
            {
                if (m.Notification.Equals(Constants.LoadingPageLoadedMsg))
                {
                    _isFromSearch = (bool) m.Sender;
                    if (_isFromSearch)
                    {
                        _searchTerm = (string) m.Target;
                    }

                    var connectionSettings = new ObjectStorageHelper<ConnectionDetails>(StorageType.Roaming);
                    //await connectionSettings.DeleteAsync(Constants.ConnectionSettings);
                    App.Settings.ConnectionDetails = await connectionSettings.LoadAsync(Constants.ConnectionSettings) ?? new ConnectionDetails {PortNo = 8096};

                    _apiClient.ServerHostName = App.Settings.ConnectionDetails.HostName;
                    _apiClient.ServerApiPort = App.Settings.ConnectionDetails.PortNo;
                    
                    _logger.Info(string.Format("Host: {0}, Port: {1}", _apiClient.ServerHostName, _apiClient.ServerApiPort));
                    
                    await CheckForServer();
                }

                if (m.Notification.Equals(Constants.ClearEverythingMsg))
                {
                    
                }
            });
        }

        private async Task<bool> CheckForServer()
        {
            if (string.IsNullOrEmpty(App.Settings.ConnectionDetails.HostName))
            {
                var result = await
                    MessageBox.ShowAsync("No hostname has been set, click OK to put one in",
                                         "Error", MessageBoxButton.OK);
                if(result == MessageBoxResult.OK)
                    SettingsPane.Show();
                ProgressText = "";
                ProgressVisibility = Visibility.Collapsed;
                return false;
            }

            var serverFound = await FindServer();
            if (!serverFound)
            {
                await MessageBox.ShowAsync("We were unable to find your server, please try again later.", "Server not found", MessageBoxButton.OK);
                return false;
            }

            var serverConfig = false;

            ProgressVisibility = Visibility.Visible;
            ProgressText = "Loading settings...";

            var settingsLoader = new ObjectStorageHelper<SpecificSettings>(StorageType.Roaming);
            //await settingsLoader.DeleteAsync(Constants.SpecificSettings);

            try
            {
                var settings = await settingsLoader.LoadAsync(Constants.SpecificSettings);
                if (settings != null)
                    await Utils.CopyItem(settings, SimpleIoc.Default.GetInstance<SpecificSettings>());
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to load settings", ex);
                settingsLoader.DeleteAsync(Constants.SpecificSettings);
            }

            //await LoadPushSettings();

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            ProgressText = "Finding server and getting configuration...";
            if (_navService.IsNetworkAvailable)
            {
                serverConfig = await GetServerConfig();

                if (serverConfig)
                {
                    var storageHelper = new ObjectStorageHelper<UserSettingWrapper>(StorageType.Roaming);
                    //await storageHelper.DeleteAsync(Constants.SelectedUserSetting);
                    var wrapper = await storageHelper.LoadAsync(Constants.SelectedUserSetting);
                    if (wrapper != null)
                    {
                        App.Settings.LoggedInUser = wrapper.User;
                        ProgressText = "Authenticating...";
                        App.Settings.PinCode = wrapper.Pin;
                        await Utils.DoLogin(_logger, App.Settings.LoggedInUser, App.Settings.PinCode,
                                          () =>
                                              {
                                                  _apiClient.CurrentUserId = App.Settings.LoggedInUser.Id;
                                                  if (_isFromSearch)
                                                  {
                                                      _navService.Navigate<SearchView>(_searchTerm);
                                                  }
                                                  else
                                                  {
                                                      _navService.Navigate<MainPage>();
                                                  }
                                              });
                    }
                    else
                    {
                        _navService.Navigate<SelectProfileView>();
                    }
                }
                else
                {
                    await
                        MessageBox.ShowAsync(
                            "There was an error getting some of the data. Please try again later.", "Error",
                            MessageBoxButton.OK);
                }
            }
            ProgressText = "";
            ProgressVisibility = Visibility.Collapsed;
            return serverConfig;
        }

        private async Task LoadPushSettings()
        {
            var storageHelper = new ObjectStorageHelper<bool>(StorageType.Local);
            var notifications = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            
            notifications.LoadingFromSettings = true;

            notifications.ServerPluginInstalled = await storageHelper.LoadAsync("ServerPluginInstalled");
            if (notifications.ServerPluginInstalled)
            {
                notifications.UseNotifications = await storageHelper.LoadAsync("UseNotifications");
                notifications.SendTileUpdates = await storageHelper.LoadAsync("SendTileUpdates");
                notifications.SendToastUpdates = await storageHelper.LoadAsync("SendToastUpdates");
                notifications.IsRegistered = await storageHelper.LoadAsync("IsRegistered");
                try
                {
                    await _apiClient.PushHeartbeatAsync(notifications.DeviceId);
                }
                catch (HttpException ex)
                {
                    _logger.Fatal("LoadPushSettings()", ex);
                }
            }
            notifications.LoadingFromSettings = false;
        } 

        private async Task<bool> FindServer()
        {
            try
            {
                _logger.Info(string.Format("FindServer() --> Connecting to {0} on port {1}.", _apiClient.ServerHostName, _apiClient.ServerApiPort));
                App.Settings.SystemStatus = await _apiClient.GetSystemInfoAsync();
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Fatal("FindServer()", ex);
                return false;
            }
        }

        private async Task<bool> GetServerConfig()
        {
            try
            {
                App.Settings.ServerConfiguration = await _apiClient.GetServerConfigurationAsync();
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Error("GetServerConfig()", ex);
                return false;
            }
        }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }

        public RelayCommand TestConnectionCommand { get; set; }
        public RelayCommand ClearConnectionSettingsCommand { get; set; }
    }
}
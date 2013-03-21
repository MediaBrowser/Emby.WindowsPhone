using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using ScottIsAFool.WindowsPhone.IsolatedStorage;
using MediaBrowser.Shared;
using MediaBrowser.Model;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SplashscreenViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient ApiClient;
        private readonly INavigationService NavigationService;
        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            ApiClient = apiClient;
            
            NavigationService = navigationService;
            if (!IsInDesignMode)
            {
                WireMessages();
                WireCommands();
            }
        }

        private void WireCommands()
        {
            TestConnectionCommand = new RelayCommand(async () =>
            {
                ProgressIsVisible = true;
                ProgressText = AppResources.SysTrayAuthenticating;
                if (NavigationService.IsNetworkAvailable)
                {
                    if (await GetServerConfiguration())
                    {
                        ISettings.DeleteValue(Constants.SelectedUserSetting);
                        ISettings.SetKeyValue(Constants.ConnectionSettings, App.Settings.ConnectionDetails);
                        await CheckProfiles();
                    }
                    else
                    {
                        App.ShowMessage("", AppResources.ErrorConnectionDetailsInvalid);
                    }
                }
                ProgressIsVisible = false;
                ProgressText = string.Empty;
            });
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.SplashAnimationFinishedMsg))
                {
                    ProgressIsVisible = true;
                    ProgressText = AppResources.SysTrayLoadingSettings;
                    // Get settings from storage
                    var connectionDetails = ISettings.GetKeyValue<ConnectionDetails>(Constants.ConnectionSettings);
                    if (connectionDetails == null)
                    {

                        //App.ShowMessage("", AppResources.ErrorNoConnectionSettings, () => NavigationService.NavigateToPage("/Views/SettingsView.xaml?settingsPane=2"));
                        App.Settings.ConnectionDetails = new ConnectionDetails
                                                             {
                                                                 PortNo = 8096
                                                             };
                        var messageBox = new CustomMessageBox
                                             {
                                                 Caption = "No connection details",
                                                 Message = "No connection settings have been set, would you like to set them now?",
                                                 LeftButtonContent = "yes",
                                                 RightButtonContent = "no",
                                                 IsFullScreen = false
                                             };
                        messageBox.Dismissed += (sender, args) =>
                                                    {
                                                        if (args.Result == CustomMessageBoxResult.LeftButton)
                                                        {
                                                            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.NavigateToPage("/Views/SettingsView.xaml?settingsPane=2"));
                                                        }
                                                    };
                        messageBox.Show();

                    }
                    else
                    {
                        App.Settings.ConnectionDetails = connectionDetails;


                        var specificSettings = ISettings.GetKeyValue<SpecificSettings>(Constants.SpecificSettings);
                        if (specificSettings != null) Utils.CopyItem(specificSettings, App.SpecificSettings);

                        var deviceName = DeviceStatus.DeviceName;
                        var deviceId = DeviceStatus.DeviceManufacturer;

                        var phone = Ailon.WP.Utils.PhoneNameResolver.Resolve(deviceId, deviceName);

                        var deviceInfo = string.Format("{0} ({1})", phone.CanonicalModel, phone.CanonicalManufacturer);

                        ApiClient.DeviceName = deviceInfo;

                        var user = ISettings.GetKeyValue<UserSettingWrapper>(Constants.SelectedUserSetting);
                        if (user != null)
                        {
                            App.Settings.LoggedInUser = user.User;
                            App.Settings.PinCode = user.Pin;
                        }
                        if (NavigationService.IsNetworkAvailable && App.Settings.ConnectionDetails != null)
                        {
                            ProgressText = AppResources.SysTrayGettingServerDetails;

                            await GetServerConfiguration();

                            if (App.Settings.ServerConfiguration != null)
                            {
                                await SetPushSettings();
                                await CheckProfiles();
                            }
                            else
                            {
                                App.ShowMessage("", AppResources.ErrorCouldNotFindServer);
                                NavigationService.NavigateToPage("/Views/SettingsView.xaml?settingsPane=2");
                            }
                        }
                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });
        }

        private async Task SetPushSettings()
        {
            var settings = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            settings.loadingFromSettings = true;

            settings.ServerPluginInstalled = ISettings.GetKeyValue<bool>("ServerPluginInstalled");

            if (settings.ServerPluginInstalled)
            {
                settings.UseNotifications = ISettings.GetKeyValue<bool>("UseNotifications");
                if (settings.UseNotifications)
                {
                    App.SpecificSettings.DeviceSettings = await ApiClient.GetDeviceSettingsAsync(settings.DeviceId);

                    settings.IsRegistered = ISettings.GetKeyValue<bool>("IsRegistered");
                    settings.SendTileUpdates = App.SpecificSettings.DeviceSettings.SendLiveTiles;
                    settings.SendToastUpdates = App.SpecificSettings.DeviceSettings.SendToasts;

                    var tilesToRemove = App.SpecificSettings.DeviceSettings.LiveTiles.Where(x => ShellTile.ActiveTiles.All(p => p.NavigationUri.ToString() != x.LiveTileId)).ToList();

                    if (tilesToRemove.Any())
                    {
                        foreach (var tile in tilesToRemove)
                        {
                            //await ApiClient.DeleteLiveTile(settings.DeviceId, tile.LiveTileId);
                        }
                        App.SpecificSettings.DeviceSettings = await ApiClient.GetDeviceSettingsAsync(settings.DeviceId);
                    }

                    settings.loadingFromSettings = false;
                    await settings.RegisterService();
                    try
                    {
                        await ApiClient.PushHeartbeatAsync(settings.DeviceId);
                    }
                    catch
                    {
                        var s = "";
                    }
                }
            }
            settings.loadingFromSettings = false;
        }

        private async Task CheckProfiles()
        {
            // If one exists, then authenticate that user.
            if (App.Settings.LoggedInUser != null)
            {
                ProgressText = AppResources.SysTrayAuthenticating;
                await Utils.Login(App.Settings.LoggedInUser, App.Settings.PinCode, () =>
                                                                                       {
                                                                                           if (!string.IsNullOrEmpty(App.Action))
                                                                                               NavigationService.NavigateToPage(App.Action);
                                                                                           else
                                                                                               NavigationService.NavigateToPage("/Views/MainPage.xaml");
                                                                                       });
            }
            else
            {
                NavigationService.NavigateToPage("/Views/ChooseProfileView.xaml");
            }
        }

        private async Task<bool> GetServerConfiguration()
        {
            try
            {
                ApiClient.ServerHostName = App.Settings.ConnectionDetails.HostName;
                ApiClient.ServerApiPort = App.Settings.ConnectionDetails.PortNo;
                var config = await ApiClient.GetServerConfigurationAsync();
                App.Settings.ServerConfiguration = config;
                return true;
            }
            catch (Exception ex)
            {
                var mbEx = ex as HttpException;
                return false;
            }
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
        public RelayCommand TestConnectionCommand { get; set; }
    }
}
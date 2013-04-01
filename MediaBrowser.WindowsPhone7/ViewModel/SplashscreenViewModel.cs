using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
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
                                                            // This is needed as a fix for the Windows Phone Toolkit giving an error.
                                                            ((CustomMessageBox)sender).Dismissing += (o, e) => e.Cancel = true;
                                                            NavigationService.NavigateToPage("/Views/SettingsView.xaml?settingsPane=2");
                                                        }
                                                    };
                        messageBox.Show();

                    }
                    else
                    {
                        App.Settings.ConnectionDetails = connectionDetails;

                        var specificSettings = ISettings.GetKeyValue<SpecificSettings>(Constants.SpecificSettings);
                        if (specificSettings != null) Utils.CopyItem(specificSettings, App.SpecificSettings);
                        
                        var user = ISettings.GetKeyValue<UserSettingWrapper>(Constants.SelectedUserSetting);
                        if (user != null)
                        {
                            App.Settings.LoggedInUser = user.User;
                            App.Settings.PinCode = user.Pin;
                        }
                        if (NavigationService.IsNetworkAvailable && App.Settings.ConnectionDetails != null)
                        {
                            ProgressText = AppResources.SysTrayGettingServerDetails;

                            await Utils.GetServerConfiguration(ApiClient);

                            if (App.Settings.ServerConfiguration != null)
                            {
                                await SetPushSettings();
                                ProgressText = AppResources.SysTrayAuthenticating;
                                await Utils.CheckProfiles(NavigationService);
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

        

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
        public RelayCommand TestConnectionCommand { get; set; }
    }
}
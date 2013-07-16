using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScottIsAFool.WindowsPhone.IsolatedStorage;
using MediaBrowser.Shared;
using MediaBrowser.Model;
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
    public class SplashscreenViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;
        private readonly IApplicationSettingsService _applicationSettings;

        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(ExtendedApiClient apiClient, INavigationService navigationService, IApplicationSettingsService applicationSettings)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _applicationSettings = applicationSettings;

            if (!IsInDesignMode)
            {
                WireCommands();
            }
        }

        private void WireCommands()
        {

        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.SplashAnimationFinishedMsg))
                {
                    SetProgressBar(AppResources.SysTrayLoadingSettings);

                    // Get settings from storage
                    var connectionDetails = _applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
                    if (connectionDetails == null)
                    {
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
                                _navigationService.NavigateTo("/Views/SettingsView.xaml?settingsPane=2");
                            }
                        };

                        messageBox.Show();
                    }
                    else
                    {
                        App.Settings.ConnectionDetails = connectionDetails;

                        // Get and set the app specific settings 
                        var specificSettings = _applicationSettings.Get<SpecificSettings>(Constants.Settings.SpecificSettings);
                        if (specificSettings != null) Utils.CopyItem(specificSettings, App.SpecificSettings);

                        // See if there is a user already saved in isolated storage
                        var user = _applicationSettings.Get<UserSettingWrapper>(Constants.Settings.SelectedUserSetting);
                        if (user != null)
                        {
                            App.Settings.LoggedInUser = user.User;
                            App.Settings.PinCode = user.Pin;
                        }

                        // See if we can find and communicate with the server
                        if (_navigationService.IsNetworkAvailable && App.Settings.ConnectionDetails != null)
                        {
                            SetProgressBar(AppResources.SysTrayGettingServerDetails);

                            await Utils.GetServerConfiguration(_apiClient, Log);

                            // Server has been found 
                            if (App.Settings.ServerConfiguration != null)
                            {
                                await SetPushSettings();
                                SetProgressBar(AppResources.SysTrayAuthenticating);
                                await Utils.CheckProfiles(_navigationService, Log);
                            }
                            else
                            {
                                App.ShowMessage(AppResources.ErrorCouldNotFindServer);
                                _navigationService.NavigateTo("/Views/SettingsView.xaml?settingsPane=2");
                            }
                        }

                        SetProgressBar();
                    }
                }
            });
        }

        private async Task SetPushSettings()
        {
            var settings = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            settings.LoadingFromSettings = true;

            settings.ServerPluginInstalled = _applicationSettings.Get<bool>(Constants.Settings.ServerPluginInstalled);

            if (settings.ServerPluginInstalled)
            {
                //settings.UseNotifications = _applicationSettings.Get<bool>(Constants.Settings.UseNotifications);
                //if (settings.UseNotifications)
                //{
                //    App.SpecificSettings.DeviceSettings = await _apiClient.GetDeviceSettingsAsync(settings.DeviceId);

                //    settings.IsRegistered = _applicationSettings.Get<bool>(Constants.Settings.IsRegistered);
                //    settings.SendTileUpdates = App.SpecificSettings.DeviceSettings.SendLiveTiles;
                //    settings.SendToastUpdates = App.SpecificSettings.DeviceSettings.SendToasts;

                //    var tilesToRemove = App.SpecificSettings.DeviceSettings.LiveTiles.Where(x => ShellTile.ActiveTiles.All(p => p.NavigationUri.ToString() != x.LiveTileId)).ToList();

                //    if (tilesToRemove.Any())
                //    {
                //        foreach (var tile in tilesToRemove)
                //        {
                //            //await ApiClient.DeleteLiveTile(settings.DeviceId, tile.LiveTileId);
                //        }
                //        App.SpecificSettings.DeviceSettings = await _apiClient.GetDeviceSettingsAsync(settings.DeviceId);
                //    }

                //    settings.LoadingFromSettings = false;
                //    await settings.RegisterService();
                //    try
                //    {
                //        await _apiClient.PushHeartbeatAsync(settings.DeviceId);
                //    }
                //    catch (HttpException ex)
                //    {
                //        Log.ErrorException("SetPushSettings()", ex);
                //    }
                //}
            }

            settings.LoadingFromSettings = false;
        }

        public RelayCommand TestConnectionCommand { get; set; }
    }
}
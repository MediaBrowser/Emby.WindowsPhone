using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using Emby.WindowsPhone.Interfaces;
using Emby.WindowsPhone.Model;
using Emby.WindowsPhone.Model.Photo;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.Model.Net;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SplashscreenViewModel : ViewModelBase
    {
        private readonly IServerInfoService _serverInfo;
        private readonly IApplicationSettingsServiceHandler _applicationSettings;

        private SpecificSettings _specificSettings;
        private UploadSettings _uploadSettings;
        private ConnectionDetails _connectionDetails;
        private ServerInfo _savedServer;

        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(IConnectionManager connectionManager, INavigationService navigationService, IApplicationSettingsService applicationSettings, IServerInfoService serverInfo)
            : base(navigationService, connectionManager)
        {
            _serverInfo = serverInfo;
            _applicationSettings = applicationSettings.Legacy;
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.SplashAnimationFinishedMsg))
                {
                    await LoadSettings();
                }
            });
        }

        private async Task LoadSettings()
        {
            RetryButtonIsVisible = false;
            App.Settings.ConnectionDetails = new ConnectionDetails
            {
                PortNo = 8096
            };

            var doNotShowFirstRun = _applicationSettings.Get(Constants.Settings.DoNotShowFirstRun, false);

            if (!doNotShowFirstRun)
            {
                NavigationService.NavigateTo(Constants.Pages.FirstRun.WelcomeView);
                return;
            }

            SetProgressBar(AppResources.SysTrayLoadingSettings);

#if !DEBUG
            //try
            //{
            //    if (!ApplicationManifest.Current.App.Title.ToLower().Contains("beta"))
            //    {
            //        var marketPlace = new MarketplaceInformationService();
            //        var appInfo = await marketPlace.GetAppInformationAsync(ApplicationManifest.Current.App.ProductId);

            //        if (new Version(appInfo.Entry.Version) > new Version(ApplicationManifest.Current.App.Version) &&
            //            MessageBox.Show("There is a newer version, would you like to install it now?", "Update Available", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        {
            //            new MarketplaceDetailService().Show();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.ErrorException("GetAppInformationAsync()", ex);
            //}
#endif
            // Get and set the app specific settings 
            _specificSettings = _applicationSettings.Get<SpecificSettings>(Constants.Settings.SpecificSettings);
            if (_specificSettings != null) _specificSettings.CopyItem(App.SpecificSettings);

            SetRunUnderLock();

            _uploadSettings = _applicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);
            if (_uploadSettings != null) _uploadSettings.CopyItem(App.UploadSettings);

            _connectionDetails = _applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
            _savedServer = _applicationSettings.Get<ServerInfo>(Constants.Settings.DefaultServerConnection);

            _serverInfo.SetServerInfo(_savedServer);

            try
            {
                await ConnectToServer();
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("ConnectToServer", ex, NavigationService, Log);
            }
            catch (OperationCanceledException exc)
            {
                var i = 1;
            }
        }

        private static void SetRunUnderLock()
        {
            //var runUnderLock = App.SpecificSettings.PlayVideosUnderLock;
            //PhoneApplicationService.Current.ApplicationIdleDetectionMode = runUnderLock ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;
        }

        private CancellationTokenSource _connectCancellationToken;

        private async Task ConnectToServer()
        {
            RetryButtonIsVisible = false;
            ConnectionResult result = null;

            SetProgressBar(AppResources.SysTrayGettingServerDetails);
            _connectCancellationToken = new CancellationTokenSource();

            if (_connectionDetails != null && !string.IsNullOrEmpty(_connectionDetails.ServerId))
            {
                result = await ConnectionManager.Connect(_connectionDetails.ServerAddress, _connectCancellationToken.Token);
                var server = result.Servers.FirstOrDefault(x =>
                        string.Equals(x.Id, _connectionDetails.ServerId, StringComparison.CurrentCultureIgnoreCase));

                if (server != null)
                {
                    _serverInfo.SetServerInfo(server);
                    _applicationSettings.Set(Constants.Settings.DefaultServerConnection, server);

                    _savedServer = server;

                    _applicationSettings.Remove(Constants.Settings.ConnectionSettings);
                    _connectionDetails = null;
                }
            }

            if (_savedServer != null)
            {
                result = await ConnectionManager.Connect(_savedServer, _connectCancellationToken.Token);
            }

            if (result != null && result.State == ConnectionState.Unavailable && _savedServer != null)
            {
                RetryButtonIsVisible = true;
                return;
            }

            // See if we can find and communicate with the server

            if (result == null || result.State == ConnectionState.Unavailable)
            {
                result = await ConnectionManager.Connect(_connectCancellationToken.Token);
            }

            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                await Utils.HandleConnectedState(result, ApiClient, NavigationService, Log);

                SetProgressBar();
            });
        }

        public bool RetryButtonIsVisible { get; set; }

        public RelayCommand RetryConnectionCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await ConnectToServer();
                });
            }
        }

        public RelayCommand ChangeServerCommand
        {
            get
            {
                return new RelayCommand(() => NavigationService.NavigateTo(Constants.Pages.SettingsViews.FindServerView));
            }
        }

        public RelayCommand GoOfflineCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_connectCancellationToken != null)
                    {
                        _connectCancellationToken.Cancel();
                    }
                });
            }
        }
    }
}
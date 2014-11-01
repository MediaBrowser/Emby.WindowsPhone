using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Photo;
using MediaBrowser.WindowsPhone.Resources;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;

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
        private readonly IApplicationSettingsService _applicationSettings;

        private SpecificSettings _specificSettings;
        private UploadSettings _uploadSettings;
        private ConnectionDetails _connectionDetails;
        private ServerInfo _savedServer;

        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(IConnectionManager connectionManager, INavigationService navigationService, IApplicationSettingsService applicationSettings)
            : base(navigationService, connectionManager)
        {
            _applicationSettings = applicationSettings;
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
            if (_specificSettings != null) Utils.CopyItem(_specificSettings, App.SpecificSettings);

            _uploadSettings = _applicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);
            if (_uploadSettings != null) Utils.CopyItem(_uploadSettings, App.UploadSettings);

            _connectionDetails = _applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
            _savedServer = _applicationSettings.Get<ServerInfo>(Constants.Settings.DefaultServerConnection);

            await ConnectToServer();
        }

        private async Task ConnectToServer()
        {
            RetryButtonIsVisible = false;
            ConnectionResult result = null;

            SetProgressBar(AppResources.SysTrayGettingServerDetails);

            if (_connectionDetails != null)
            {
                result = await ConnectionManager.Connect(_connectionDetails.ServerAddress, default(CancellationToken));
                var server = result.Servers.FirstOrDefault(x =>
                        string.Equals(x.LocalAddress, _connectionDetails.ServerAddress, StringComparison.CurrentCultureIgnoreCase)
                        || string.Equals(x.RemoteAddress, _connectionDetails.ServerAddress, StringComparison.CurrentCultureIgnoreCase));

                if (server != null)
                {
                    _applicationSettings.Set(Constants.Settings.DefaultServerConnection, server);
                    _applicationSettings.Save();

                    _savedServer = server;

                    _applicationSettings.Reset(Constants.Settings.ConnectionSettings);
                    _connectionDetails = null;
                }
            }

            if (_savedServer != null)
            {
                result = await ConnectionManager.Connect(_savedServer, default(CancellationToken));
            }

            if (result != null && result.State == ConnectionState.Unavailable && _savedServer != null)
            {
                RetryButtonIsVisible = true;
                return;
            }

            // See if we can find and communicate with the server

            if (result == null || result.State == ConnectionState.Unavailable)
            {
                result = await ConnectionManager.Connect(default(CancellationToken));
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
    }
}
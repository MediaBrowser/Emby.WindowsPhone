using System.Threading;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Photo;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using Microsoft.Phone.Controls;
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
                    ConnectionResult result = null;

                    // Get settings from storage 
                    var connectionDetails = _applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
                    if (connectionDetails != null)
                    {
                        result = await ConnectionManager.Connect(connectionDetails.ServerAddress, default(CancellationToken));
                    }

                    var savedServer = _applicationSettings.Get<ServerInfo>(Constants.Settings.DefaultServerConnection);
                    if (savedServer != null)
                    {
                        result = await ConnectionManager.Connect(savedServer, default(CancellationToken));
                    }

                    // Get and set the app specific settings 
                    var specificSettings = _applicationSettings.Get<SpecificSettings>(Constants.Settings.SpecificSettings);
                    if (specificSettings != null) Utils.CopyItem(specificSettings, App.SpecificSettings);

                    var uploadSettings = _applicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);
                    if (uploadSettings != null) Utils.CopyItem(uploadSettings, App.UploadSettings);

                    // See if we can find and communicate with the server
                    SetProgressBar(AppResources.SysTrayGettingServerDetails);

                    if (result == null || result.State == ConnectionState.Unavailable)
                    {
                        result = await ConnectionManager.Connect(default(CancellationToken));
                    }

                    Deployment.Current.Dispatcher.BeginInvoke(async () =>
                    {
                        switch (result.State)
                        {
                            case ConnectionState.Unavailable:
                                App.ShowMessage(AppResources.ErrorCouldNotFindServer);
                                NavigationService.NavigateTo(Constants.Pages.FirstRun.MbConnectFirstRunView);
                                break;
                            case ConnectionState.ServerSelection:
                                NavigationService.NavigateTo(Constants.Pages.SettingsViews.FindServerView);
                                break;
                            case ConnectionState.ServerSignIn:
                                await Utils.CheckProfiles(NavigationService, Log, ApiClient);
                                break;
                            case ConnectionState.SignedIn:
                                if (AuthenticationService.Current.LoggedInUser == null)
                                {
                                    var user = await ApiClient.GetUserAsync(ApiClient.CurrentUserId);
                                    AuthenticationService.Current.SetUser(user);
                                }

                                await Utils.StartEverything(NavigationService, Log, ApiClient);

                                NavigationService.NavigateTo(Constants.Pages.MainPage);
                                break;
                        }

                        SetProgressBar();    
                    });
                    
                }
            });
        }

        public RelayCommand TestConnectionCommand { get; set; }
    }
}
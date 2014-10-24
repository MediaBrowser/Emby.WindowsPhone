using System;
using System.Windows;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Photo;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Controls;
using MediaBrowser.Model;
using ScottIsAFool.WindowsPhone.ViewModel;
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
        private readonly IExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;
        private readonly IApplicationSettingsService _applicationSettings;

        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(IExtendedApiClient apiClient, INavigationService navigationService, IApplicationSettingsService applicationSettings)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
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
                        _navigationService.NavigateTo(Constants.Pages.FirstRun.WelcomeView);
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

                    // Get settings from storage 
                    var connectionDetails = _applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
                    if (connectionDetails == null)
                    {
                        var messageBox = new CustomMessageBox
                        {
                            Caption = AppResources.ErrorConnectionDetailsTitle,
                            Message = AppResources.ErrorConnectionDetailsMessage,
                            LeftButtonContent = AppResources.LabelYes,
                            RightButtonContent = AppResources.LabelNo,
                            IsFullScreen = false
                        };

                        messageBox.Dismissed += (sender, args) =>
                        {
                            if (args.Result == CustomMessageBoxResult.LeftButton)
                            {
                                _navigationService.NavigateTo(Constants.Pages.SettingsViewConnection);
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

                        var uploadSettings = _applicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);
                        if (uploadSettings != null) Utils.CopyItem(uploadSettings, App.UploadSettings);
                        
                        // See if we can find and communicate with the server
                        if (_navigationService.IsNetworkAvailable && App.Settings.ConnectionDetails != null)
                        {
                            SetProgressBar(AppResources.SysTrayGettingServerDetails);

                            await Utils.GetServerConfiguration(_apiClient, Log);

                            // Server has been found 
                            if (App.Settings.SystemStatus != null)
                            {
                                SetProgressBar(AppResources.SysTrayAuthenticating);
                                await Utils.CheckProfiles(_navigationService, Log, _apiClient);
                            }
                            else
                            {
                                App.ShowMessage(AppResources.ErrorCouldNotFindServer);
                                _navigationService.NavigateTo(Constants.Pages.SettingsViewConnection);
                            }
                        }

                        SetProgressBar();
                    }
                }
            });
        }

        public RelayCommand TestConnectionCommand { get; set; }
    }
}
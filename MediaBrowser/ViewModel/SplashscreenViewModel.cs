using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.WindowsPhone.Model;
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
        private readonly ApiClient ApiClient;
        private readonly INavigationService NavigationService;
        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(ApiClient apiClient, INavigationService navigationService)
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
                ProgressText = "Checking connection...";
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
                        App.ShowMessage("", "Connection details are invalid, please try again.");
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
                    // Get settings from storage
                    var connectionDetails = ISettings.GetKeyValue<ConnectionDetails>(Constants.ConnectionSettings);
                    if (connectionDetails != null)
                    {
                        App.Settings.ConnectionDetails = connectionDetails;
                    }
                    else
                    {
                        App.ShowMessage("", "No connection settings, tap to set", () => NavigationService.NavigateToPage("/Views/Settings/ConnectionSettings.xaml"));
                        App.Settings.ConnectionDetails = new ConnectionDetails
                                                             {
                                                                 PortNo = 8096
                                                             };
                        return;
                    }



                    var user = ISettings.GetKeyValue<UserSettingWrapper>(Constants.SelectedUserSetting);
                    if (user != null)
                    {
                        App.Settings.LoggedInUser = user.User;
                        App.Settings.PinCode = user.Pin;
                    }
                    if (NavigationService.IsNetworkAvailable && App.Settings.ConnectionDetails != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting server details...";

                        await GetServerConfiguration();

                        if (App.Settings.ServerConfiguration != null)
                        {
                            await CheckProfiles();
                        }
                        else
                        {
                            App.ShowMessage("", "Could not find your server.");
                        }
                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });
        }

        private async Task CheckProfiles()
        {
            // If one exists, then authenticate that user.
            if (App.Settings.LoggedInUser != null)
            {
                ProgressText = "Authenticating...";
                await Utils.Login(App.Settings.LoggedInUser, App.Settings.PinCode, () => NavigationService.NavigateToPage("/Views/MainPage.xaml"));
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
            catch
            {
                return false;
            }
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
        public RelayCommand TestConnectionCommand { get; set; }
    }
}
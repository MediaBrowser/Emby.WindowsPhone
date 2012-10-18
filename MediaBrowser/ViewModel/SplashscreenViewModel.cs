using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.IsolatedStorage;
using MediaBrowser.Model.DTO;
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
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m=>
            {
                if(m.Notification.Equals(Constants.SplashAnimationFinishedMsg))
                {
                    // Get settings from storage
                    var connectionDetails = ISettings.GetKeyValue<ConnectionDetails>(Constants.ConnectionSettings);
                    if(connectionDetails != null)
                    {
                        App.Settings.ConnectionDetails = connectionDetails;
                    }
                    else
                    {
                        App.ShowMessage("", "No connection settings, tap to set", () => NavigationService.NavigateToPage("/Views/Settings/Connection.xaml"));
                        //return;
                    }
                    App.Settings.ConnectionDetails = new ConnectionDetails
                                                         {
                                                             HostName = "192.168.0.2",
                                                             PortNo = 8096
                                                         };

                    ApiClient.ServerHostName = App.Settings.ConnectionDetails.HostName;
                    ApiClient.ServerApiPort = App.Settings.ConnectionDetails.PortNo;
                    var user = ISettings.GetKeyValue<UserSettingWrapper>(Constants.SelectedUserSetting);
                    if(user != null)
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
                            // If one exists, then authenticate that user.
                            if (App.Settings.LoggedInUser != null)
                            {
                                ProgressText = "Authenticating...";
                                await Utils.Login(App.Settings.LoggedInUser, App.Settings.PinCode, () => NavigationService.NavigateToPage("/Views/MainPage.xaml"));
                            }
                            else
                            {
                                // As no user is saved, check whether users are used at all
                                // If they are, get the user to check what profile is wanted.
                                if (App.Settings.ServerConfiguration.EnableUserProfiles)
                                {
                                    NavigationService.NavigateToPage("/Views/ChooseProfileView.xaml");
                                }
                                else
                                {
                                    // If not, get the default user and log them in.
                                    ProgressText = "Getting default user...";
                                    App.Settings.LoggedInUser = await ApiClient.GetDefaultUserAsync();
                                    if (App.Settings.LoggedInUser != null)
                                    {
                                        NavigationService.NavigateToPage("/Views/MainPage.xaml");
                                    }
                                    else
                                    {
                                        App.ShowMessage("", "No default user found.");
                                    }
                                }
                            }
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

        private async Task GetServerConfiguration()
        {
            var config = await ApiClient.GetServerConfigurationAsync();
            App.Settings.ServerConfiguration = config;
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
    }
}
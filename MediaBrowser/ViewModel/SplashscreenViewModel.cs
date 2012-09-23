using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.WindowsPhone.Model;

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
                    App.Settings.HostName = "192.168.0.2";
                    App.Settings.PortNo = 8096;

                    ApiClient.ServerHostName = App.Settings.HostName;
                    ApiClient.ServerApiPort = App.Settings.PortNo;
                    if (NavigationService.IsNetworkAvailable)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting server details...";

                        await GetServerConfiguration();

                        if (App.Settings.ServerConfiguration != null)
                        {
                            if (App.Settings.ServerConfiguration.EnableUserProfiles &&
                                App.Settings.LoggedInUser == null)
                            {
                                NavigationService.NavigateToPage("/Views/ChooseProfileView.xaml");
                            }
                            else
                            {
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
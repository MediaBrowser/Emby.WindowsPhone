using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using PropertyChanged;

namespace MediaBrowser.Services
{
    [ImplementPropertyChanged]
    public class AuthenticationService
    {
        private static AuthenticationService _current;
        private static readonly IApplicationSettingsService SettingsService = new ApplicationSettingsService();
        private static IExtendedApiClient _apiClient;
        private static ILogger _logger;

        public static AuthenticationService Current
        {
            get { return _current ?? (_current = new AuthenticationService()); }
        }

        public void Start(IExtendedApiClient apiClient, ILogger logger)
        {
            _apiClient = apiClient;
            _logger = logger;

            CheckIfUserSignedIn();
        }

        private void CheckIfUserSignedIn()
        {
            var user = SettingsService.Get<UserDto>(Constants.Settings.SelectedUserSetting);

            if (user != null)
            {
                LoggedInUser = user;
                IsLoggedIn = true;
            }
        }

        public async Task Login(string selectedUserName, string pinCode)
        {
            try
            {
                _logger.Info("Authenticating user [{0}]", selectedUserName);

                var result = await _apiClient.AuthenticateUserAsync(selectedUserName, pinCode.ToHash());

                _logger.Info("Logged in as [{0}]", selectedUserName);

                LoggedInUser = result.User;
                IsLoggedIn = true;

                SettingsService.Set(Constants.Settings.SelectedUserSetting, LoggedInUser);
                SettingsService.Save();
                _logger.Info("User [{0}] has been saved", selectedUserName);
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("Login()", ex);
            }
        }

        public void Logout()
        {
            LoggedInUser = null;
            IsLoggedIn = false;

            SettingsService.Reset(Constants.Settings.SelectedUserSetting);
            SettingsService.Save();
        }

        public UserDto LoggedInUser { get; set; }
        public bool IsLoggedIn { get; private set; }
    }
}

using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Users;
using PropertyChanged;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Services
{
    [ImplementPropertyChanged]
    public class AuthenticationService
    {
        private readonly IConnectionManager _connectionManager;
        private IApplicationSettingsService _settingsService;
        private static IApiClient _apiClient;
        private static ILog _logger;

        public AuthenticationResult AuthenticationResult { get; set; }

        public static AuthenticationService Current { get; private set; }

        public AuthenticationService(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _logger = new WPLogger(typeof (AuthenticationService));
            Current = this;
        }

        public void Start()
        {
            _settingsService = new ApplicationSettingsService();
            _apiClient = _connectionManager.GetApiClient(null);

            CheckIfUserSignedIn();
        }

        public void CheckIfUserSignedIn()
        {
            var user = _settingsService.Get<AuthenticationResult>(Constants.Settings.SelectedUserSetting);

            if (user != null && !string.IsNullOrEmpty(user.AccessToken))
            {
                AuthenticationResult = user;
                IsLoggedIn = true;
                SetAuthenticationInfo();
            }
        }

        public async Task Login(string selectedUserName, string pinCode)
        {
            try
            {
                _logger.Info("Authenticating user [{0}]", selectedUserName);

                var result = await _apiClient.AuthenticateUserAsync(selectedUserName, pinCode);

                _logger.Info("Logged in as [{0}]", selectedUserName);

                AuthenticationResult = result;
                IsLoggedIn = true;
                SetAuthenticationInfo();

                _settingsService.Set(Constants.Settings.SelectedUserSetting, result);
                _settingsService.Save();
                _logger.Info("User [{0}] has been saved", selectedUserName);
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("Login()", ex);
            }
        }

        public void SetAuthenticationInfo()
        {
            _apiClient.ClearAuthenticationInfo();
            _apiClient.SetAuthenticationInfo(AuthenticationResult.AccessToken, LoggedInUserId);
        }

        public async Task SignOut()
        {
            try
            {
                await _apiClient.Logout();
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("SignOut()", ex);
            }
        }

        public void Logout()
        {
            IsLoggedIn = false;

            _settingsService.Reset(Constants.Settings.SelectedUserSetting);
            _settingsService.Save();
        }

        public UserDto LoggedInUser
        {
            get { return AuthenticationResult != null ? AuthenticationResult.User : null; }
        }

        public bool IsLoggedIn { get; private set; }

        public string LoggedInUserId
        {
            get
            {
                return LoggedInUser != null ? LoggedInUser.Id : null;
            }
        }
    }
}

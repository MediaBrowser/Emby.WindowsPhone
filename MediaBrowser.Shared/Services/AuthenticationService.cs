using System;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using PropertyChanged;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.Services
{
    [ImplementPropertyChanged]
    public class AuthenticationService
    {
        private static AuthenticationService _current;
        private static readonly IApplicationSettingsService SettingsService = new ApplicationSettingsService();
        private static ExtendedApiClient _apiClient;
        private static ILog _logger;

        public AuthenticationService()
        {
            _logger = new WPLogger(typeof(AuthenticationService));
        }

        public static AuthenticationService Current
        {
            get { return _current ?? (_current = new AuthenticationService()); }
        }

        public void Start(ExtendedApiClient apiClient)
        {
            _apiClient = apiClient;

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

        public void Stop()
        {
            
        }

        public async Task LogIn(UserDto selectedUser, string pinCode)
        {
            try
            {
                _logger.Info("Authenticating user [{0}]", selectedUser.Name);

                await _apiClient.AuthenticateUserAsync(selectedUser.Id, pinCode.ToHash());

                _logger.Info("Logged in as [{0}]", selectedUser.Name);

                LoggedInUser = selectedUser;
                IsLoggedIn = true;

                SettingsService.Set(Constants.Settings.SelectedUserSetting, selectedUser);
                SettingsService.Save();
                _logger.Info("User [{0}] has been saved", selectedUser.Name);
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("Utils.Login()", ex);
            }
        }

        public void LogOut()
        {
            LoggedInUser = null;
            IsLoggedIn = false;
        }

        public UserDto LoggedInUser { get; set; }
        public bool IsLoggedIn { get; private set; }
    }
}

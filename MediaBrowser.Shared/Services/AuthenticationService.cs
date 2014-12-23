using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Connect;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Events;
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
        private static ILog _logger;

        public AuthenticationResult AuthenticationResult { get; set; }

        public static AuthenticationService Current { get; private set; }

        public AuthenticationService(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _logger = new WPLogger(typeof (AuthenticationService));
            Current = this;

            _connectionManager.ConnectUserSignIn += ConnectionManagerOnConnectUserSignIn;
            _connectionManager.ConnectUserSignOut += ConnectionManagerOnConnectUserSignOut;
            _connectionManager.LocalUserSignIn += ConnectionManagerOnLocalUserSignIn;
            _connectionManager.LocalUserSignOut += ConnectionManagerOnLocalUserSignOut;
        }

        private void ConnectionManagerOnLocalUserSignOut(object sender, EventArgs eventArgs)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoggedInUser = null);
        }

        private void ConnectionManagerOnLocalUserSignIn(object sender, GenericEventArgs<UserDto> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                LoggedInUser = e.Argument;
                if (AuthenticationResult != null && _connectionManager.CurrentApiClient != null)
                {
                    _connectionManager.CurrentApiClient.SetAuthenticationInfo(AuthenticationResult.AccessToken, LoggedInUserId);
                }
            });
        }

        private void ConnectionManagerOnConnectUserSignOut(object sender, EventArgs eventArgs)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoggedInConnectUser = null);
        }

        private void ConnectionManagerOnConnectUserSignIn(object sender, GenericEventArgs<ConnectUser> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoggedInConnectUser = e.Argument);
        }

        public void Start()
        {
            _settingsService = new ApplicationSettingsService();
            CheckIfUserSignedIn();
        }

        public void CheckIfUserSignedIn()
        {
            var user = _settingsService.Get<UserDto>(Constants.Settings.SelectedUserSetting);
            var oldUser = _settingsService.Get<AuthenticationResult>(Constants.Settings.AuthUserSetting);

            if (user != null)
            {
                LoggedInUser = user;
            }

            if (oldUser != null)
            {
                AuthenticationResult = oldUser;
            }
        }

        public async Task Login(string selectedUserName, string pinCode)
        {
            try
            {
                _logger.Info("Authenticating user [{0}]", selectedUserName);

                var result = await _connectionManager.CurrentApiClient.AuthenticateUserAsync(selectedUserName, pinCode);

                _logger.Info("Logged in as [{0}]", selectedUserName);

                AuthenticationResult = result;
                _settingsService.Set(Constants.Settings.AuthUserSetting, AuthenticationResult);
                _settingsService.Save();

                SetUser(result.User);
                _logger.Info("User [{0}] has been saved", selectedUserName);
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("Login()", ex);
            }
        }

        public void SetAuthenticationInfo()
        {
            if (AuthenticationResult != null && AuthenticationResult.User != null && !string.IsNullOrEmpty(AuthenticationResult.User.Id))
            {
                _connectionManager.CurrentApiClient.ClearAuthenticationInfo();
                _connectionManager.CurrentApiClient.SetAuthenticationInfo(AuthenticationResult.AccessToken, AuthenticationResult.User.Id);
            }
        }

        public void ClearLoggedInUser()
        {
            LoggedInUser = null;
            _settingsService.Reset(Constants.Settings.SelectedUserSetting);
            _settingsService.Save();
        }

        public async Task SignOut()
        {
            try
            {
                await _connectionManager.Logout();
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("SignOut()", ex);
            }
        }

        public void Logout()
        {
            LoggedInUser = null;
            LoggedInConnectUser = null;
            AuthenticationResult = null;
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ClearNowPlayingMsg));

            _settingsService.Reset(Constants.Settings.SelectedUserSetting);
            _settingsService.Reset(Constants.Settings.AuthUserSetting);
            _settingsService.Reset(Constants.Settings.DefaultServerConnection);
            _settingsService.Save();
        }

        public UserDto LoggedInUser { get; private set; }

        public bool IsLoggedIn { get { return LoggedInUser != null; } }

        public string LoggedInUserId
        {
            get { return LoggedInUser != null ? LoggedInUser.Id : null; }
        }

        public bool SignedInUsingConnect { get { return LoggedInConnectUser != null && LoggedInUser != null && LoggedInConnectUser.Id == LoggedInUser.ConnectUserId; } }

        public ConnectUser LoggedInConnectUser { get; private set; }

        public async Task<bool> LoginWithConnect(string username, string password)
        {
            try
            {
                await _connectionManager.LoginToConnect(username, password);
                return true;
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("Error logging into MB Connect", ex);
                return false;
            }
            catch (WebException wex)
            {
                _logger.ErrorException("Error logging into MB Connect", wex);
                return false;
            }
            catch (Exception eex)
            {
                _logger.ErrorException("Error logging into MB Connect", eex);
                return false;
            }
        }

        public void SetUser(UserDto user)
        {
            LoggedInUser = user;

            _settingsService.Set(Constants.Settings.SelectedUserSetting, LoggedInUser);
            _settingsService.Save();
        }
    }
}

using System.Threading;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel.Settings
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MbConnectViewModel : ViewModelBase
    {
        private readonly IApplicationSettingsServiceHandler _appSettings;

        /// <summary>
        /// Initializes a new instance of the MbConnectViewModel class.
        /// </summary>
        public MbConnectViewModel(INavigationService navigationService, IConnectionManager connectionManager, IApplicationSettingsService appSettings)
            : base(navigationService, connectionManager)
        {
            _appSettings = appSettings.Legacy;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public bool CanSignIn
        {
            get
            {
                return !string.IsNullOrEmpty(Username)
                       && !string.IsNullOrEmpty(Password)
                       && !ProgressIsVisible;
            }
        }

        public RelayCommand SignInCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!CanSignIn)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayAuthenticating);

                    try
                    {
                        var success = await AuthenticationService.Current.LoginWithConnect(Username, Password);

                        if (success)
                        {
                            var result = await ConnectionManager.Connect(default(CancellationToken));

                            if (result.State == ConnectionState.SignedIn && result.Servers.Count == 1)
                            {
                                App.ServerInfo = result.Servers[0];
                                _appSettings.Set(Constants.Settings.DefaultServerConnection, result.Servers[0]);
                                _appSettings.Save();
                            }

                            await Utils.HandleConnectedState(result, ApiClient, NavigationService, Log);
                        }
                        else
                        {
                            App.ShowMessage(AppResources.ErrorSigningIn);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("SignInWithConnectCommand", ex, NavigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand LogOutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    AuthenticationService.Current.SignOut().ConfigureAwait(false);

                    NavigationService.NavigateTo(Constants.Pages.FirstRun.MbConnectFirstRunView, true);

                    AuthenticationService.Current.Logout();
                });
            }
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanSignIn);
        }
    }
}
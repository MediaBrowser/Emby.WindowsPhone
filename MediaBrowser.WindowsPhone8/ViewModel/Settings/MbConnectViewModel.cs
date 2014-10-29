using System.Threading;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;

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
        /// <summary>
        /// Initializes a new instance of the MbConnectViewModel class.
        /// </summary>
        public MbConnectViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            
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
                            await ConnectionManager.Connect(default(CancellationToken));
                            var user = await ApiClient.GetUserAsync(ApiClient.CurrentUserId);
                            AuthenticationService.Current.SetUser(user);
                            NavigationService.NavigateTo(Constants.Pages.MainPage, true);
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

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanSignIn);
        }
    }
}
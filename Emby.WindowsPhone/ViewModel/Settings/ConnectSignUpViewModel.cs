using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Services;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using Org.BouncyCastle.Asn1.Cms;

namespace Emby.WindowsPhone.ViewModel.Settings
{
    public class ConnectSignUpViewModel : ViewModelBase
    {
        public ConnectSignUpViewModel(INavigationService navigationService, IConnectionManager connectionManager) 
            : base(navigationService, connectionManager)
        {
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }

        public string ErrorMessage { get; set; }

        public bool DisplayErrorMessage
        {
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }

        public bool CanSignUp
        {
            get
            {
                return !ProgressIsVisible
                       && !string.IsNullOrEmpty(Username)
                       && !string.IsNullOrEmpty(Password)
                       && !string.IsNullOrEmpty(EmailAddress);
            }
        }

        public RelayCommand SignUpCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!CanSignUp)
                    {
                        return;
                    }

                    ErrorMessage = string.Empty;
                    SetProgressBar(AppResources.SysTraySigningUp);

                    try
                    {
                        var response = await AuthenticationService.Current.SignUpForConnect(EmailAddress, Username, Password);
                        if (response.IsSuccessful)
                        {
                            NavigationService.NavigateTo(Constants.Pages.SettingsViews.MbConnectView);
                        }
                        else if (response.IsEmailInUse)
                        {
                            ErrorMessage = AppResources.ErrorEmailInUse;
                        }
                        else if (response.IsUsernameInUse)
                        {
                            ErrorMessage = AppResources.ErrorUsernameInUse;
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("SignUpCommand", ex, NavigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanSignUp);
            base.UpdateProperties();
        }
    }
}

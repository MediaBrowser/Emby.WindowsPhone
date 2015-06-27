using Cimbalino.Toolkit.Services;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.ViewModel.Settings
{
    public class ConnectSignUpViewModel : ViewModelBase
    {
        private readonly IMessageBoxService _messageBox;

        public ConnectSignUpViewModel(INavigationService navigationService, IConnectionManager connectionManager, IMessageBoxService messageBox)
            : base(navigationService, connectionManager)
        {
            _messageBox = messageBox;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }

        public PasswordScore PasswordScore
        {
            get { return Password.CheckStrength(); }
        }

        public bool IsValidUsername
        {
            get { return !string.IsNullOrEmpty(Username)
                       && !Username.Contains(" "); }
        }

        public bool IsValidPassword
        {
            get
            {
                return !string.IsNullOrEmpty(Password)
                       && Password.Length >= 8;
            }
        }

        public bool IsValidEmailAddress
        {
            get
            {
                return !string.IsNullOrEmpty(EmailAddress)
                       && EmailAddress.IsValidEmail();
            }
        }

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
                       && IsValidUsername
                       && IsValidPassword
                       && IsValidEmailAddress;
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
                        switch (response)
                        {
                            case ConnectSignupResponse.Success:
                                await _messageBox.ShowAsync(AppResources.MessageSignUpSuccessful, AppResources.MessageTitleSuccess, new[] {"Ok"});
                                NavigationService.NavigateTo(Constants.Pages.SettingsViews.MbConnectView);
                                Reset();
                                break;
                            case ConnectSignupResponse.EmailInUse:
                                ErrorMessage = AppResources.ErrorEmailInUse;
                                break;
                            case ConnectSignupResponse.UsernameInUser:
                                ErrorMessage = AppResources.ErrorUsernameInUse;
                                break;
                            default:
                                ErrorMessage = AppResources.ErrorSigningUp;
                                break;
                        }
                    }
                    catch (HttpException ex)
                    {
                        ErrorMessage = AppResources.ErrorSigningUp;
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

        private void Reset()
        {
            ErrorMessage = Username = EmailAddress = Password = string.Empty;
        }
    }
}

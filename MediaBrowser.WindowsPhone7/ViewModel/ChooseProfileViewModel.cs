using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.ViewModel;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChooseProfileViewModel : ViewModelBase
    {
        private readonly IExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the ChooseProfileViewModel class.
        /// </summary>
        public ChooseProfileViewModel(IExtendedApiClient apiClient, INavigationService navigationService, IApplicationSettingsService applicationSettings)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

            Profiles = new ObservableCollection<UserDto>();
            if (IsInDesignMode)
            {
                Profiles = new ObservableCollection<UserDto>
                {
                    new UserDto
                    {
                        Id = new Guid("dd425709431649698e92d86b1f2b00fa").ToString(),
                        Name = "ScottIsAFool"
                    },
                    new UserDto
                    {
                        Id = new Guid("dab28e40cfbc43658082f55a44cf139a").ToString(),
                        Name = "Redshirt",
                        LastLoginDate = DateTime.Now.AddHours(-1)
                    }
                };
            }
            else
            {
                WireCommands();
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public bool CanLogin
        {
            get
            {
                return true;// !string.IsNullOrEmpty(Username);
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ResetAppMsg))
                {
                    Profiles.Clear();
                }
            });
        }

        private void WireCommands()
        {
            ChooseProfilePageLoaded = new RelayCommand(async () =>
            {
                if (_navigationService.IsNetworkAvailable)
                {
                    SetProgressBar(AppResources.SysTrayGettingProfiles);

                    Log.Info("Getting profiles");

                    try
                    {
                        var profiles = await _apiClient.GetPublicUsersAsync();
                        foreach (var profile in profiles)
                        {
                            Profiles.Add(profile);
                        }

                        if (!Profiles.Any())
                        {
                            _navigationService.NavigateTo(Constants.Pages.ManualUsernameView);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "GettingProfiles()", _navigationService, Log);
                    }

                    SetProgressBar();
                }
            });

            LoginCommand = new RelayCommand<object[]>(async loginDetails =>
            {
                var selectedUser = loginDetails[0] as string;
                var pinCode = loginDetails[1] as string;
                
                await DoLogin(selectedUser, pinCode);
            });

            ManualLoginCommand = new RelayCommand(async () =>
            {
                await DoLogin(Username, Password);
            });
        }

        private async Task DoLogin(string selectedUserName, string pinCode)
        {
            if (string.IsNullOrEmpty(selectedUserName))
            {
                return;
            }

            Debug.WriteLine(selectedUserName);

            SetProgressBar(AppResources.SysTrayAuthenticating);

            await AuthenticationService.Current.Login(selectedUserName, pinCode);
            if (AuthenticationService.Current.IsLoggedIn)
            {
                _navigationService.NavigateTo(!string.IsNullOrEmpty(App.Action) ? App.Action : Constants.Pages.HomePage);
                Username = Password = string.Empty;
                _apiClient.CurrentUserId = AuthenticationService.Current.LoggedInUser.Id;
            }
            else
            {
                MessageBox.Show("We were unable to sign you in at this time, this could be due to an incorrect username/password, or your account has been disabled.", "Login unsuccessful", MessageBoxButton.OK);
            }
            
            SetProgressBar();
        }

        public ObservableCollection<UserDto> Profiles { get; set; }

        public RelayCommand ChooseProfilePageLoaded { get; set; }
        public RelayCommand<object[]> LoginCommand { get; set; }
        public RelayCommand ManualLoginCommand { get; set; }
    }
}
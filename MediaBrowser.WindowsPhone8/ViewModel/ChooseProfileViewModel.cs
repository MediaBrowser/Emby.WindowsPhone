using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;

using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;

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
        /// <summary>
        /// Initializes a new instance of the ChooseProfileViewModel class.
        /// </summary>
        public ChooseProfileViewModel(IConnectionManager connectionManager, INavigationService navigationService, IApplicationSettingsService applicationSettings)
            : base(navigationService, connectionManager)
        {
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
                if (NavigationService.IsNetworkAvailable)
                {
                    SetProgressBar(AppResources.SysTrayGettingProfiles);

                    Log.Info("Getting profiles");

                    try
                    {
                        var profiles = await ApiClient.GetPublicUsersAsync(new CancellationToken());
                        Profiles = new ObservableCollection<UserDto>();

                        foreach (var profile in profiles)
                        {
                            Profiles.Add(profile);
                        }

                        if (!Profiles.Any())
                        {
                            NavigationService.NavigateTo(Constants.Pages.ManualUsernameView);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "GettingProfiles()", NavigationService, Log);
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
                await Utils.StartEverything(NavigationService, Log, ApiClient);
                var page = TileService.Current.PinnedPage();
                NavigationService.NavigateTo(page, true);
                Username = Password = string.Empty;
            }
            else
            {
                MessageBox.Show(AppResources.ErrorUnableToSignIn, AppResources.ErrorUnableToSignInTitle, MessageBoxButton.OK);
            }
            
            SetProgressBar();
        }

        public ObservableCollection<UserDto> Profiles { get; set; }

        public RelayCommand ChooseProfilePageLoaded { get; set; }
        public RelayCommand<object[]> LoginCommand { get; set; }
        public RelayCommand ManualLoginCommand { get; set; }
    }
}
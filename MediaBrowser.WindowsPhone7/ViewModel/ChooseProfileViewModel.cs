using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
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
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;
        private readonly IApplicationSettingsService _applicationSettings;

        /// <summary>
        /// Initializes a new instance of the ChooseProfileViewModel class.
        /// </summary>
        public ChooseProfileViewModel(ExtendedApiClient apiClient, INavigationService navigationService, IApplicationSettingsService applicationSettings)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _applicationSettings = applicationSettings;

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
                        var profiles = await _apiClient.GetUsersAsync(new UserQuery());
                        foreach (var profile in profiles)
                            Profiles.Add(profile);
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("GettingProfiles()", ex);
                    }

                    SetProgressBar();
                }
            });

            LoginCommand = new RelayCommand<object[]>(async loginDetails =>
            {
                var selectedUser = loginDetails[0] as UserDto;
                var pinCode = loginDetails[1] as string;
                var saveUser = (bool) loginDetails[2];

                if (selectedUser != null)
                {
                    Debug.WriteLine(selectedUser.Id);

                    SetProgressBar(AppResources.SysTrayAuthenticating);

                    await Utils.Login(Log, selectedUser, pinCode, () =>
                    {
                        SetUser(selectedUser);
                        if (saveUser)
                        {
                            _applicationSettings.Set(Constants.Settings.SelectedUserSetting, new UserSettingWrapper
                            {
                                User = selectedUser,
                                Pin = pinCode
                            });
                            _applicationSettings.Save();
                            Log.Info("User [{0}] has been saved", selectedUser.Name);
                        }
                    });

                    SetProgressBar();
                }
            });
        }

        private void SetUser(UserDto profile)
        {
            App.Settings.LoggedInUser = profile;
            if (!string.IsNullOrEmpty(App.Action))
            {
                _navigationService.NavigateTo(App.Action);
            }
            else
            {
                _navigationService.NavigateTo("/Views/MainPage.xaml");
            }
        }

        public ObservableCollection<UserDto> Profiles { get; set; }

        public RelayCommand ChooseProfilePageLoaded { get; set; }
        public RelayCommand<object[]> LoginCommand { get; set; }
    }
}
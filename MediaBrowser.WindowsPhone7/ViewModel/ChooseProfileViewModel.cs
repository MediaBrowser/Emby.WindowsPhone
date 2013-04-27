using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.IsolatedStorage;
using MediaBrowser.Shared;

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
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the ChooseProfileViewModel class.
        /// </summary>
        public ChooseProfileViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _logger = new WPLogger(typeof(ChooseProfileViewModel));
            Profiles = new ObservableCollection<UserDto>();
            if(IsInDesignMode)
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
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.ResetAppMsg))
                                                                          {
                                                                              Profiles.Clear();
                                                                          }
                                                                      });
        }

        private void WireCommands()
        {
            ChooseProfilePageLoaded = new RelayCommand(async ()=>
            {
                if(_navigationService.IsNetworkAvailable)
                {
                    ProgressText = AppResources.SysTrayGettingProfiles;
                    ProgressIsVisible = true;

                    _logger.Log("Getting profiles");
                    
                    try
                    {
                        var profiles = await _apiClient.GetAllUsersAsync();
                        foreach (var profile in profiles)
                            Profiles.Add(profile);
                    }
                    catch (HttpException ex)
                    {
                        _logger.Log(ex.Message, LogLevel.Fatal);
                        _logger.Log(ex.StackTrace, LogLevel.Fatal);
                    }

                    ProgressText = string.Empty;
                    ProgressIsVisible = false;
                }
            });

            LoginCommand = new RelayCommand<object[]>(async loginDetails =>
            {
                var selectedUser = loginDetails[0] as UserDto;
                var pinCode = loginDetails[1] as string;
                var saveUser = (bool)loginDetails[2];

                if (selectedUser != null)
                {
                    Debug.WriteLine(selectedUser.Id);

                    ProgressText = AppResources.SysTrayAuthenticating;
                    ProgressIsVisible = true;

                    await Utils.Login(_logger, selectedUser, pinCode, () =>
                    {
                        SetUser(selectedUser);
                        if(saveUser)
                        {
                            ISettings.SetKeyValue(Constants.SelectedUserSetting, new UserSettingWrapper
                                                                                     {
                                                                                         User = selectedUser,
                                                                                         Pin = pinCode
                                                                                     });
                            _logger.LogFormat("User [{0}] has been saved", LogLevel.Info, selectedUser.Name);
                        }
                    });

                    ProgressText = "";
                    ProgressIsVisible = false;
                }
            });
        }

        private void SetUser(UserDto profile)
        {
            App.Settings.LoggedInUser = profile;
            if (!string.IsNullOrEmpty(App.Action))
                _navigationService.NavigateTo(App.Action);
            else
                _navigationService.NavigateTo("/Views/MainPage.xaml");
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public ObservableCollection<UserDto> Profiles { get; set; }

        public RelayCommand ChooseProfilePageLoaded { get; set; }
        public RelayCommand<object[]> LoginCommand { get; set; }
    }
}
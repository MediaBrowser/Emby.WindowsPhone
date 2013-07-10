using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MediaBrowser.Windows8.Views;
using MetroLog;
using WinRtUtility;
using Windows.UI.Xaml;
using MediaBrowser.Model.Dto;
using MediaBrowser.Shared;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SelectProfilesViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;
        private bool _haveProfiles;

        /// <summary>
        /// Initializes a new instance of the ProfilesViewModel class.
        /// </summary>
        public SelectProfilesViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
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
                                           Name = "ScottIsAFool",
                                           Id = "5d1cf7fce25943b790d140095457a42b"
                                       },
                                   new UserDto
                                       {
                                           Name = "Redshirt",
                                           Id = Guid.NewGuid().ToString()
                                       }
                               };
                _logger = new DesignLogger();
            }
            else
            {
                WireCommands();
                WireMessages();
                _logger = LogManagerFactory.DefaultLogManager.GetLogger<SelectProfilesViewModel>();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.ProfileViewLoadedMsg))
                {
                    if (_navigationService.IsNetworkAvailable)
                    {
                        ProgressText = "Getting profiles";
                        ProgressVisibility = Visibility.Visible;

                        _haveProfiles = await GetUserProfiles();

                        ProgressText = "";
                        ProgressVisibility = Visibility.Collapsed;
                    }
                }
                if (m.Notification.Equals(Constants.DoLoginMsg))
                {
                    var loginObjects = m.Sender as object[];
                    var selectedUser = loginObjects[0] as UserDto;
                    var pinCode = loginObjects[1] as string;
                    var saveUser = (bool)loginObjects[2];

                    if (selectedUser != null)
                    {

                        Debug.WriteLine(selectedUser.Id);
                        ProgressText = "Authenticating...";
                        ProgressVisibility = Visibility.Visible;

                        await Utils.DoLogin(_logger, selectedUser, pinCode, async () =>
                        {
                            SetUser(selectedUser);
                            if(saveUser)
                            {
                                var storageHelper = new ObjectStorageHelper<UserSettingWrapper>(StorageType.Roaming);
                                await storageHelper.SaveAsync(new UserSettingWrapper{ User = selectedUser, Pin = pinCode}, Constants.SelectedUserSetting);

                                _logger.Info("User [{0}] has been saved", selectedUser.Name);
                            }
                        });

                        ProgressText = "";
                        ProgressVisibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private async Task<bool> GetUserProfiles()
        {
            try
            {
                _logger.Info("Getting profiles");
                var profiles = await _apiClient.GetUsersAsync();

                Profiles.Clear();

                foreach(var profile in profiles)
                {
                    Profiles.Add(profile);
                }
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Fatal(ex.Message, ex);
                return false;
            }
        }

        private void WireCommands()
        {
            ProfileTappedCommand = new RelayCommand<UserDto>(profile =>
            {
                if (profile.HasPassword)
                {
                    return;
                }
                SetUser(profile);
            });

            ChangeProfileCommand = new RelayCommand(async () =>
            {
                _logger.Info("Signed out user [{0}]", App.Settings.LoggedInUser.Name);

                App.Settings.LoggedInUser = null;
                App.Settings.PinCode = string.Empty;

                Messenger.Default.Send(new NotificationMessage(Constants.ClearEverythingMsg));

                History.Current.ClearAll();

                await new ObjectStorageHelper<UserSettingWrapper>(StorageType.Roaming).DeleteAsync(Constants.SelectedUserSetting);

                _logger.Info("Signed out.");

                _navigationService.Navigate<SelectProfileView>();
            });
        }

        private void SetUser(UserDto profile)
        {
            App.Settings.LoggedInUser = profile;
            _apiClient.CurrentUserId = profile.Id;
            _navigationService.Navigate<MainPage>();
        }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }
        
        public ObservableCollection<UserDto> Profiles { get; set; }

        public RelayCommand<UserDto> ProfileTappedCommand { get; set; }
        public RelayCommand ChangeProfileCommand { get; set; }
    }
}
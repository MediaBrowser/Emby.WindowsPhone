using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Windows8.Model;
using MediaBrowser.Windows8.Views;
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
        private readonly ExtendedApiClient ApiClient;
        private readonly NavigationService NavigationService;
        private bool haveProfiles;

        /// <summary>
        /// Initializes a new instance of the ProfilesViewModel class.
        /// </summary>
        public SelectProfilesViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            Profiles = new ObservableCollection<UserDto>();
            if (IsInDesignMode)
            {
                Profiles = new ObservableCollection<UserDto>
                               {
                                   new UserDto
                                       {
                                           Name = "ScottIsAFool",
                                           Id = new Guid("5d1cf7fce25943b790d140095457a42b")
                                       },
                                   new UserDto
                                       {
                                           Name = "Redshirt",
                                           Id = Guid.NewGuid()
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
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.ProfileViewLoadedMsg))
                {
                    if (NavigationService.IsNetworkAvailable)
                    {
                        ProgressText = "Getting profiles";
                        ProgressVisibility = Visibility.Visible;

                        haveProfiles = await GetUserProfiles();

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

                        await Utils.DoLogin(selectedUser, pinCode, async () =>
                        {
                            SetUser(selectedUser);
                            if(saveUser)
                            {
                                var storageHelper = new ObjectStorageHelper<UserSettingWrapper>(StorageType.Roaming);
                                await storageHelper.SaveAsync(new UserSettingWrapper{ User = selectedUser, Pin = pinCode}, Constants.SelectedUserSetting);
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
                var profiles = await ApiClient.GetAllUsersAsync();
                Profiles.Clear();
                foreach(var profile in profiles)
                {
                    Profiles.Add(profile);
                }
                return true;
            }
            catch
            {
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
                App.Settings.LoggedInUser = null;
                App.Settings.PinCode = string.Empty;
                Messenger.Default.Send(new NotificationMessage(Constants.ClearEverythingMsg));
                History.Current.ClearAll();
                await new ObjectStorageHelper<UserSettingWrapper>(StorageType.Roaming).DeleteAsync(Constants.SelectedUserSetting);
                NavigationService.Navigate<SelectProfileView>();
            });
        }

        private void SetUser(UserDto profile)
        {
            App.Settings.LoggedInUser = profile;
            ApiClient.CurrentUserId = profile.Id;
            NavigationService.Navigate<MainPage>();
        }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }
        
        public ObservableCollection<UserDto> Profiles { get; set; }

        public RelayCommand<UserDto> ProfileTappedCommand { get; set; }
        public RelayCommand ChangeProfileCommand { get; set; }
    }
}
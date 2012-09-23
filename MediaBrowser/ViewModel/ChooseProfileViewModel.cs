using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model.DTO;

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
        private readonly ApiClient ApiClient;
        private readonly INavigationService NavigationService;

        /// <summary>
        /// Initializes a new instance of the ChooseProfileViewModel class.
        /// </summary>
        public ChooseProfileViewModel(ApiClient apiClient, INavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            Profiles = new ObservableCollection<DtoUser>();
            if(IsInDesignMode)
            {
                Profiles = new ObservableCollection<DtoUser>()
                               {
                                   new DtoUser
                                       {
                                           HasImage = true,
                                           Id = new Guid("dd425709431649698e92d86b1f2b00fa"),
                                           Name = "ScottIsAFool"
                                       },
                                   new DtoUser
                                       {
                                           HasImage = true,
                                           Id = new Guid("dab28e40cfbc43658082f55a44cf139a"),
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

        private void WireCommands()
        {
            ChooseProfilePageLoaded = new RelayCommand(async ()=>
            {
                if(NavigationService.IsNetworkAvailable)
                {
                    var profiles = await ApiClient.GetAllUsersAsync();
                    foreach(var profile in profiles)
                        Profiles.Add(profile);
                }
            });
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public ObservableCollection<DtoUser> Profiles { get; set; }

        public RelayCommand ChooseProfilePageLoaded { get; set; }
    }
}
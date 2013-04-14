using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model;
#if !WP8
using ScottIsAFool.WindowsPhone;
#endif

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TrailerViewModel : ViewModelBase
    {
        private readonly INavigationService NavigationService;
        private readonly ExtendedApiClient ApiClient;
        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        public TrailerViewModel(INavigationService navigation, ExtendedApiClient apiClient)
        {
            NavigationService = navigation;
            ApiClient = apiClient;
            if (IsInDesignMode)
            {
                SelectedTrailer = new BaseItemDto
                                      {
                                          Name = "Jurassic Park 3D",
                                          Overview =
                                              "Universal Pictures will release Steven Spielberg\u2019s groundbreaking masterpiece JURASSIC PARK in 3D on April 5, 2013.  With his remastering of the epic into a state-of-the-art 3D format, Spielberg introduces the three-time Academy Award\u00AE-winning blockbuster to a new generation of moviegoers and allows longtime fans to experience the world he envisioned in a way that was unimaginable during the film\u2019s original release.  Starring Sam Neill, Laura Dern, Jeff Goldblum, Samuel L. Jackson and Richard Attenborough, the film based on the novel by Michael Crichton is produced by Kathleen Kennedy and Gerald R. Molen.",
                                          PremiereDate = DateTime.Parse("2013-04-05T00:00:00.0000000"),
                                          Id = "4aed3d79a0c4c2a0ac9c91fb7a641f1a",
                                          ProductionYear = 2013,
                                          People = new[]
                                                       {
                                                           new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                                                           new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                                                           new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                                                           new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                                                       }
                                      };
                CastAndCrew = Utils.GroupCastAndCrew(SelectedTrailer.People);
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
                                                                          if (m.Notification.Equals(Constants.ChangeTrailerMsg))
                                                                          {
                                                                              SelectedTrailer = (BaseItemDto)App.SelectedItem;
                                                                          }
                                                                      });
        }

        private void WireCommands()
        {
            TrailerPageLoaded = new RelayCommand(async () =>
                                                     {
                                                         if (NavigationService.IsNetworkAvailable)
                                                         {
                                                             ProgressText = "Getting trailer details...";
                                                             ProgressIsVisible = true;

                                                             SelectedTrailer = await ApiClient.GetItemAsync(SelectedTrailer.Id, App.Settings.LoggedInUser.Id);
                                                             
                                                             CastAndCrew = Utils.GroupCastAndCrew(SelectedTrailer.People);

                                                             ProgressText = string.Empty;
                                                             ProgressIsVisible = false;
                                                         }
                                                         else
                                                         {
                                                             // TODO: Message
                                                         }
                                                     });
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public BaseItemDto SelectedTrailer { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }

        public RelayCommand TrailerPageLoaded { get; set; }
    }
}
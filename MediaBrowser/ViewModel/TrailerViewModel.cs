using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.DTO;
using MediaBrowser.WindowsPhone.Model;

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
        private readonly ApiClient ApiClient;
        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        public TrailerViewModel(INavigationService navigation, ApiClient apiClient)
        {
            NavigationService = navigation;
            ApiClient = apiClient;
            if (IsInDesignMode)
            {
                SelectedTrailer = new DtoBaseItem
                {
                    Name = "Jurassic Park 3D",
                    Overview =
                        "Universal Pictures will release Steven Spielberg\u2019s groundbreaking masterpiece JURASSIC PARK in 3D on April 5, 2013.  With his remastering of the epic into a state-of-the-art 3D format, Spielberg introduces the three-time Academy Award\u00AE-winning blockbuster to a new generation of moviegoers and allows longtime fans to experience the world he envisioned in a way that was unimaginable during the film\u2019s original release.  Starring Sam Neill, Laura Dern, Jeff Goldblum, Samuel L. Jackson and Richard Attenborough, the film based on the novel by Michael Crichton is produced by Kathleen Kennedy and Gerald R. Molen.",
                    PremiereDate = DateTime.Parse("2013-04-05T00:00:00.0000000"),
                    Id = "4aed3d79a0c4c2a0ac9c91fb7a641f1a",
                    ProductionYear = 2013
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
                                                                          if (m.Notification.Equals(Constants.ChangeTrailerMsg))
                                                                          {
                                                                              SelectedTrailer = (DtoBaseItem)App.SelectedItem;
                                                                          }
                                                                      });
        }

        private void WireCommands()
        {
            TrailerPageLoaded = new RelayCommand(async () =>
                                                     {
                                                         if (NavigationService.IsNetworkAvailable)
                                                         {
                                                             SelectedTrailer = await ApiClient.GetItemAsync(SelectedTrailer.Id, App.Settings.LoggedInUser.Id);
                                                         }
                                                         else
                                                         {
                                                             // TODO: Message
                                                         }
                                                     });
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public DtoBaseItem SelectedTrailer { get; set; }

        public RelayCommand TrailerPageLoaded { get; set; }
    }
}
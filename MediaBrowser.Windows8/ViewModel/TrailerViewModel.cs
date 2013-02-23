using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Dto;
using MediaBrowser.Windows8.Model;
using Windows.UI.Xaml;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TrailerViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient ApiClient;
        private readonly NavigationService NavigationService;
        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        public TrailerViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            if (IsInDesignMode)
            {
                CastAndCrew = new ObservableCollection<Group<BaseItemPerson>>
                              {
                                  new Group<BaseItemPerson> {Title = "Director"},
                                  new Group<BaseItemPerson> {Title = "Cast"}
                              };
                SelectedTrailer = new BaseItemDto
                                      {
                                          Name = "Jurassic Park 3D",
                                          Overview =
                                              "Universal Pictures will release Steven Spielberg\u2019s groundbreaking masterpiece JURASSIC PARK in 3D on April 5, 2013.  With his remastering of the epic into a state-of-the-art 3D format, Spielberg introduces the three-time Academy Award\u00AE-winning blockbuster to a new generation of moviegoers and allows longtime fans to experience the world he envisioned in a way that was unimaginable during the film\u2019s original release.  Starring Sam Neill, Laura Dern, Jeff Goldblum, Samuel L. Jackson and Richard Attenborough, the film based on the novel by Michael Crichton is produced by Kathleen Kennedy and Gerald R. Molen.",
                                              PremiereDate = DateTime.Parse("2013-04-05T00:00:00.0000000"),
                                          Id = "4aed3d79a0c4c2a0ac9c91fb7a641f1a",
                                          ProductionYear = 2013,
                                      };

                CastAndCrew[0].Items.Add(new BaseItemPerson { Name = "Steven Spielberg", Type = "Director" });
                CastAndCrew[1].Items.Add(new BaseItemPerson{Name = "Sam Neill", Type = "Actor"});
                CastAndCrew[1].Items.Add(new BaseItemPerson { Name = "Richard Attenborough", Type = "Actor" });
            }
            else
            {
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.ChangeTrailerMsg))
                                                                          {
                                                                              SelectedTrailer = (BaseItemDto) m.Sender;
                                                                              CastAndCrew = new ObservableCollection<Group<BaseItemPerson>>();
                                                                          }
                                                                          if (m.Notification.Equals(Constants.TrailerPageLoadedMsg))
                                                                          {
                                                                              if (SelectedTrailer != null)
                                                                              {
                                                                                  if (NavigationService.IsNetworkAvailable)
                                                                                  {
                                                                                      ProgressText = "Getting trailer details...";
                                                                                      ProgressVisibility = Visibility.Visible;

                                                                                      var trailerDetails = await GetTrailerDetails();

                                                                                      var query = new VideoStreamOptions
                                                                                      {
                                                                                          ItemId = SelectedTrailer.Id,
                                                                                          VideoCodec = VideoCodecs.H264,
                                                                                          OutputFileExtension = "ts",
                                                                                          AudioCodec = AudioCodecs.Mp3,
                                                                                          MaxHeight = 768,
                                                                                          MaxWidth = 1366
                                                                                      };
                                                                                      TrailerUrl = ApiClient.GetVideoStreamUrl(query);

                                                                                      CastAndCrew = await Utils.GroupCastAndCrew(SelectedTrailer);

                                                                                      ProgressText = string.Empty;
                                                                                      ProgressVisibility = Visibility.Collapsed;
                                                                                  }
                                                                              }
                                                                          }
                                                                      });
        }

        private async Task<bool> GetTrailerDetails()
        {
            try
            {
                SelectedTrailer = await ApiClient.GetItemAsync(SelectedTrailer.Id, App.Settings.LoggedInUser.Id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Visibility ProgressVisibility { get; set; }
        public string ProgressText { get; set; }

        public BaseItemDto SelectedTrailer { get; set; }
        public string TrailerUrl { get; set; }
        public ObservableCollection<Group<BaseItemPerson>> CastAndCrew { get; set; }
    }
}
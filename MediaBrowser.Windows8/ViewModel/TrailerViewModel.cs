using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MetroLog;
using Windows.System.Display;
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
        private readonly ExtendedApiClient _apiClient;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;

        private DisplayRequest _displayRequest;
        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        public TrailerViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

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

                _logger = new DesignLogger();
            }
            else
            {
                WireMessages();
                _logger = LogManagerFactory.DefaultLogManager.GetLogger<TrailerViewModel>();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.Messages.ChangeTrailerMsg))
                                                                          {
                                                                              SelectedTrailer = (BaseItemDto) m.Sender;
                                                                              CastAndCrew = new ObservableCollection<Group<BaseItemPerson>>();
                                                                          }
                                                                          if (m.Notification.Equals(Constants.Messages.TrailerPageLoadedMsg))
                                                                          {
                                                                              if (SelectedTrailer != null)
                                                                              {
                                                                                  if (_navigationService.IsNetworkAvailable)
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

                                                                                      TrailerUrl = _apiClient.GetVideoStreamUrl(query);

                                                                                      SetDisplayTimeout();

                                                                                      _logger.Debug(TrailerUrl);

                                                                                      CastAndCrew = await Utils.GroupCastAndCrew(SelectedTrailer);

                                                                                      ProgressText = string.Empty;
                                                                                      ProgressVisibility = Visibility.Collapsed;
                                                                                  }
                                                                              }
                                                                          }
                                                                          if (m.Notification.Equals(Constants.Messages.LeftTrailerMsg))
                                                                          {
                                                                              if (_displayRequest != null)
                                                                              {
                                                                                  _displayRequest.RequestRelease();
                                                                              }
                                                                          }
                                                                      });
        }

        private void SetDisplayTimeout()
        {
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
                _displayRequest.RequestActive();
            }
        }

        private async Task<bool> GetTrailerDetails()
        {
            try
            {
                _logger.Info("Getting information for trailer [{0}] ({1})", SelectedTrailer.Name, SelectedTrailer.Id);

                SelectedTrailer = await _apiClient.GetItemAsync(SelectedTrailer.Id, App.Settings.LoggedInUser.Id);
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Fatal(ex.Message, ex);
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
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
    public class VideoPlayerViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient ApiClient;
        private readonly INavigationService NavigationService;

        private bool isResume;
        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            if (!IsInDesignMode)
            {
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.PlayVideoItemMsg))
                                                                          {
                                                                              if (m.Sender != null)
                                                                              {
                                                                                  SelectedItem = (DtoBaseItem) m.Sender;
                                                                                  if (m.Target != null)
                                                                                      isResume = (bool) m.Target;
                                                                              }
                                                                          }
                                                                          if (m.Notification.Equals(Constants.SendVideoTimeToServerMsg))
                                                                          {
                                                                              try
                                                                              {
                                                                                  var totalTicks = isResume ? StartTime.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;
                                                                                  SelectedItem.UserData = await ApiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id, totalTicks);
                                                                              }
                                                                              catch
                                                                              {
                                                                                  string v = "v";
                                                                              }
                                                                          }
                                                                      });
        }

        public string VideoUrl { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan PlayedVideoDuration { get; set; }
        public DtoBaseItem SelectedItem { get; set; }

        public RelayCommand VideoPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                                            {
                                                long ticks = 0;
                                                if (SelectedItem.UserData != null && isResume)
                                                {
                                                    ticks = SelectedItem.UserData.PlaybackPositionTicks;
                                                }
                                                StartTime = TimeSpan.FromTicks(ticks);
                                                var query = new VideoStreamOptions
                                                {
                                                    ItemId = SelectedItem.Id,
                                                    VideoCodec = VideoCodecs.H264,
                                                    OutputFileExtension = "ts",
                                                    AudioCodec = AudioCodecs.Mp3,
                                                    StartTimeTicks = ticks,
                                                    MaxHeight = 480,
                                                    MaxWidth = 800
                                                };

                                                VideoUrl = ApiClient.GetVideoStreamUrl(query);
                                                Debug.WriteLine(VideoUrl);

                                                try
                                                {
                                                    await ApiClient.ReportPlaybackStartAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id);
                                                }
                                                catch{}
                                            });
            }
        }

    }
}
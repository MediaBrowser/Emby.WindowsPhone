using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.DTO;

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
        private readonly ApiClient ApiClient;
        private DtoBaseItem selectedItem;
        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(ApiClient apiClient)
        {
            ApiClient = apiClient;
            if (!IsInDesignMode)
            {
                WireCommands();
                WireMessages();
            }
        }

        private void WireCommands()
        {
            VideoPageLoaded = new RelayCommand(() =>
                                                   {

                                                   });
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.PlayVideoItemMsg))
                                                                          {
                                                                              selectedItem = (DtoBaseItem)m.Sender;
                                                                              
                                                                              // Ask permission to not lock the screen.

                                                                              var query = new VideoStreamOptions
                                                                              {
                                                                                  ItemId = selectedItem.Id,
                                                                                  OutputFileExtension= "ts",
                                                                                  VideoCodec = VideoCodecs.H264,
                                                                                  AudioCodec = AudioCodecs.Mp3,
                                                                                  MaxHeight = 480,
                                                                                  MaxWidth = 800,
                                                                                  
                                                                              };
                                                                              VideoUrl = ApiClient.GetVideoStreamUrl(query);
                                                                          }
                                                                      });
        }

        public string VideoUrl { get; set; }

        public RelayCommand VideoPageLoaded { get; set; }
    }
}
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction.WindowsPhone;
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
                                                                              var formats = new List<VideoOutputFormats>
                                                                                                {
                                                                                                    VideoOutputFormats.Wmv,
                                                                                                    VideoOutputFormats.Asf,
                                                                                                    VideoOutputFormats.Ts
                                                                                                };
                                                                              // Ask permission to not lock the screen.
                                                                              VideoUrl = ApiClient.GetVideoStreamUrl(selectedItem.Id, formats, maxHeight: 480, maxWidth: 800, quality: StreamingQuality.Higher);
                                                                          }
                                                                      });
        }

        public string VideoUrl { get; set; }

        public RelayCommand VideoPageLoaded { get; set; }
    }
}
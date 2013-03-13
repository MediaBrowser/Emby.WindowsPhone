using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Dto;
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
        private BaseItemDto selectedItem;
        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(ExtendedApiClient apiClient)
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
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.PlayVideoItemMsg))
                                                                          {
                                                                              selectedItem = (BaseItemDto)m.Sender;
                                                                              
                                                                              // Ask permission to not lock the screen.

                                                                              var bounds = Application.Current.RootVisual.RenderSize;
                                                                              var query = new VideoStreamOptions
                                                                              {
                                                                                  ItemId = selectedItem.Id,
                                                                                  VideoCodec = VideoCodecs.Wmv,
                                                                                  //OutputFileExtension = "ts",
                                                                                  AudioCodec = AudioCodecs.Mp3,
                                                                                  MaxHeight = (int)bounds.Width,
                                                                                  MaxWidth = (int)bounds.Height
                                                                              };
                                                                              //try
                                                                              //{
                                                                              //    await ApiClient.ReportPlaybackStartAsync(selectedItem.Id, App.Settings.LoggedInUser.Id).ConfigureAwait(true);
                                                                              //}
                                                                              //catch
                                                                              //{
                                                                              //    var s = "";
                                                                              //}
                                                                              VideoUrl = new Uri(ApiClient.GetVideoStreamUrl(query));
                                                                          }
                                                                      });
        }

        public Uri VideoUrl { get; set; }

        public RelayCommand VideoPageLoaded { get; set; }
    }
}
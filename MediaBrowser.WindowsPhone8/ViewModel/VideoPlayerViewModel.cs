using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.ViewModel;

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
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        private bool _isResume;

        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.PlayVideoItemMsg))
                {
                    if (m.Sender != null)
                    {
                        SelectedItem = (BaseItemDto) m.Sender;
                        if (m.Target != null)
                        {
                            _isResume = (bool) m.Target;
                        }
                    }
                }

                if (m.Notification.Equals(Constants.SendVideoTimeToServerMsg))
                {
                    try
                    {
                        var totalTicks = _isResume && StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;

                        Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                        await _apiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id, totalTicks);
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("SendVideoTimeToServer", ex);
                    }
                }
            });
        }

        public string VideoUrl { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan PlayedVideoDuration { get; set; }
        public BaseItemDto SelectedItem { get; set; }

        public RelayCommand VideoPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    long ticks = 0;
                    if (SelectedItem.UserData != null && _isResume)
                    {
                        ticks = SelectedItem.UserData.PlaybackPositionTicks;
                    }
                    StartTime = TimeSpan.FromTicks(ticks);
                    var query = new VideoStreamOptions
                    {
                        ItemId = SelectedItem.Id,
                        VideoCodec = VideoCodecs.H264,
                        OutputFileExtension = ".mp4",
                        AudioCodec = AudioCodecs.Aac,
                        VideoBitRate = 1000000,
                        AudioBitRate = 128000,
                        MaxAudioChannels = 2,
                        Profile = "baseline",
                        Level = "3",
                        //FrameRate = 20,
                        MaxHeight = 480, // (int)bounds.Width,
                        MaxWidth = 800 // (int)bounds.Height
                    };

                    VideoUrl = _apiClient.GetVideoStreamUrl(query);
                    Debug.WriteLine(VideoUrl);

                    Log.Info("Playing {0} [{1}] ({2})", SelectedItem.Type, SelectedItem.Name, SelectedItem.Id);
                    Log.Debug(VideoUrl);

                    try
                    {
                        Log.Info("Sending playback started message to the server.");
                        await _apiClient.ReportPlaybackStartAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id);
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("VideoPageLoaded", ex);
                    }
                });
            }
        }

    }
}
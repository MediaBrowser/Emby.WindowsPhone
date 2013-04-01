using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
                        SelectedItem = (BaseItemDto)m.Sender;
                        if (m.Target != null)
                            isResume = (bool)m.Target;
                    }
                }
                if (m.Notification.Equals(Constants.SendVideoTimeToServerMsg))
                {
                    try
                    {
                        var totalTicks = isResume ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                        await ApiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id, totalTicks);
                    }
                    catch
                    {
                        string v = "v";
                    }
                }
                if (m.Notification.Equals(Constants.SendVideoTimeToServerMsg))
                {
                    try
                    {
                        var totalTicks = StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                        await ApiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id, totalTicks);
                    }
                    catch
                    {
                        var v = "v";
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
                    if (SelectedItem.UserData != null && isResume)
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
                        //FrameRate = 20,
                        MaxHeight = 480,// (int)bounds.Width,
                        MaxWidth = 800// (int)bounds.Height
                    };

                    VideoUrl = ApiClient.GetVideoStreamUrl(query);
                    Debug.WriteLine(VideoUrl);

                    try
                    {
                        await ApiClient.ReportPlaybackStartAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id);
                    }
                    catch { }
                });
            }
        }

    }
}
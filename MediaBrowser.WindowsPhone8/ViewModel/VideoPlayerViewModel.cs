using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Messaging;
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
        private readonly IExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        private readonly DispatcherTimer _timer;

        private bool _isResume;
        private long? _startPositionTicks;

        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(IExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _timer = new DispatcherTimer{Interval = TimeSpan.FromSeconds(10)};
            _timer.Tick += TimerOnTick;
        }

        private async void TimerOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                var totalTicks = _isResume && StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;

                Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                await _apiClient.ReportPlaybackProgressAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId, totalTicks, false, false);
                SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("TimerOnTick()", ex);
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<VideoMessage>(this, m =>
            {
                if (m.VideoItem != null)
                {
                    SelectedItem = m.VideoItem;
                    _isResume = m.IsResume;
                    _startPositionTicks = m.ResumeTicks;
                }
            });

            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.PlayVideoItemMsg))
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

                if (m.Notification.Equals(Constants.Messages.SetResumeMsg))
                {
                    _isResume = true;
                }

                if (m.Notification.Equals(Constants.Messages.SendVideoTimeToServerMsg))
                {
                    try
                    {
                        var totalTicks = _isResume && StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;

                        Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                        await _apiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId, totalTicks);
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;

                        if (_timer != null && _timer.IsEnabled)
                        {
                            _timer.Stop();
                        }
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("SendVideoTimeToServer", ex);
                    }
                }

                if (m.Notification.Equals(Constants.Messages.VideoStateChangedMsg))
                {
                    try
                    {
                        var totalTicks = _isResume && StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;
                        var isPaused = m.Sender != null && (bool) m.Sender;

                        if (_timer != null)
                        {
                            if (isPaused)
                            {
                                if (_timer.IsEnabled)
                                {
                                    _timer.Stop();
                                }
                            }
                            else
                            {
                                if (!_timer.IsEnabled)
                                {
                                    _timer.Start();
                                }
                            }
                        }

                        Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                        await _apiClient.ReportPlaybackProgressAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId, totalTicks, isPaused, false);
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("VideoStateChanged", ex);
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
                    if (SelectedItem.VideoType != VideoType.VideoFile)
                    {
                        var result = MessageBox.Show("Playing this type of video is currently experimental, results may vary, do you wish to continue?", "Experimental", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.Cancel)
                        {
                            _navigationService.GoBack();
                            return;
                        }
                    }

                    long ticks = 0;
                    if (SelectedItem.UserData != null && _isResume)
                    {
                        ticks = SelectedItem.UserData.PlaybackPositionTicks;
                    }
                    
                    var query = new VideoStreamOptions
                    {
                        ItemId = SelectedItem.Id,
                        VideoCodec = VideoCodecs.H264,
                        OutputFileExtension = ".mp4",
                        AudioCodec = AudioCodecs.Aac,
                        VideoBitRate = 1000000,
                        AudioBitRate = 128000,
                        MaxAudioChannels = 2,
                        StartTimeTicks = ticks,
                        Profile = "baseline",
                        Level = "3",
                        //FrameRate = 20,
                        MaxHeight = 480, // (int)bounds.Width,
                        MaxWidth = 800 // (int)bounds.Height
                    };

                    VideoUrl = _apiClient.GetVideoStreamUrl(query);
                    Debug.WriteLine(VideoUrl);

                    StartTime = TimeSpan.FromTicks(ticks);

                    if (_timer != null && !_timer.IsEnabled)
                    {
                        _timer.Start();
                    }

                    Log.Info("Playing {0} [{1}] ({2})", SelectedItem.Type, SelectedItem.Name, SelectedItem.Id);
                    Log.Debug(VideoUrl);

                    try
                    {
                        Log.Info("Sending playback started message to the server.");
                        await _apiClient.ReportPlaybackStartAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId, false, new List<string>());
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
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
        private PlayerSourceType _playerSourceType;
        private string _itemId;

        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(IExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            _timer.Tick += TimerOnTick;
        }

        private async void TimerOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                var totalTicks = _isResume && StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;

                Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                var info = new PlaybackProgressInfo
                {
                    ItemId = _itemId,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    IsMuted = false,
                    IsPaused = false,
                    PositionTicks = totalTicks
                };

                await _apiClient.ReportPlaybackProgressAsync(info);
                SetPlaybackTicks(totalTicks);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("TimerOnTick()", ex);
            }
        }

        private void SetPlaybackTicks(long totalTicks)
        {
            switch (_playerSourceType)
            {
                case PlayerSourceType.Video:
                    SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                    break;
                case PlayerSourceType.Recording:
                    RecordingItem.UserData.PlaybackPositionTicks = totalTicks;
                    break;
                case PlayerSourceType.Programme:
                    ProgrammeItem.UserData.PlaybackPositionTicks = totalTicks;
                    break;
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<VideoMessage>(this, m =>
            {
                switch (m.PlayerSourceType)
                {
                    case PlayerSourceType.Video:
                        if (m.VideoItem != null)
                        {
                            SelectedItem = m.VideoItem;
                        }
                        break;
                    case PlayerSourceType.Recording:
                        if (m.RecordingItem != null)
                        {
                            RecordingItem = m.RecordingItem;
                        }
                        break;
                    case PlayerSourceType.Programme:
                        if (m.ProgrammeItem != null)
                        {
                            ProgrammeItem = m.ProgrammeItem;
                        }
                        break;
                }

                _playerSourceType = m.PlayerSourceType;
                _isResume = m.IsResume;
                _startPositionTicks = m.ResumeTicks;
            });

            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
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

                        var info = new PlaybackStopInfo
                        {
                            ItemId = _itemId,
                            UserId = AuthenticationService.Current.LoggedInUserId,
                            PositionTicks = totalTicks
                        };

                        await _apiClient.ReportPlaybackStoppedAsync(info);
                        
                        SetPlaybackTicks(totalTicks);

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
                        var isPaused = m.Sender != null && (bool)m.Sender;

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

                        var info = new PlaybackProgressInfo
                        {
                            IsMuted = false,
                            ItemId = _itemId,
                            UserId = AuthenticationService.Current.LoggedInUserId,
                            PositionTicks = totalTicks,
                            IsPaused = isPaused
                        };

                        await _apiClient.ReportPlaybackProgressAsync(info);
                        SetPlaybackTicks(totalTicks);
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
        public RecordingInfoDto RecordingItem { get; set; }
        public ProgramInfoDto ProgrammeItem { get; set; }

        public RelayCommand VideoPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var query = new VideoStreamOptions();
                    long ticks = 0;
                    switch (_playerSourceType)
                    {
                        case PlayerSourceType.Video:
                            if (SelectedItem.VideoType != VideoType.VideoFile)
                            {
                                var result = MessageBox.Show(AppResources.MessageExperimentalVideo, AppResources.MessageExperimentalTitle, MessageBoxButton.OKCancel);
                                if (result == MessageBoxResult.Cancel)
                                {
                                    _navigationService.GoBack();
                                    return;
                                }
                            }

                            query = CreateVideoStreamOptions(SelectedItem.Id, SelectedItem.UserData, ref ticks);
                            Log.Info("Playing {0} [{1}] ({2})", SelectedItem.Type, SelectedItem.Name, SelectedItem.Id);
                            break;
                        case PlayerSourceType.Recording:
                            query = CreateVideoStreamOptions(RecordingItem.Id, RecordingItem.UserData, ref ticks);
                            Log.Info("Playing {0} [{1}] ({2})", RecordingItem.Type, RecordingItem.Name, RecordingItem.Id);
                            break;
                        case PlayerSourceType.Programme:
                            query = CreateVideoStreamOptions(ProgrammeItem.Id, ProgrammeItem.UserData, ref ticks);
                            Log.Info("Playing {0} [{1}] ({2})", ProgrammeItem.Type, ProgrammeItem.Name, ProgrammeItem.Id);
                            break;
                    }

                    VideoUrl = _apiClient.GetVideoStreamUrl(query);
                    Debug.WriteLine(VideoUrl);

                    StartTime = TimeSpan.FromTicks(ticks);

                    if (_timer != null && !_timer.IsEnabled)
                    {
                        _timer.Start();
                    }

                    Log.Debug(VideoUrl);

                    try
                    {
                        Log.Info("Sending playback started message to the server.");

                        var info = new PlaybackStartInfo
                        {
                            ItemId = query.ItemId,
                            UserId = AuthenticationService.Current.LoggedInUserId,
                            IsSeekable = false,
                            QueueableMediaTypes = new string[0]
                        };

                        await _apiClient.ReportPlaybackStartAsync(info);
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("VideoPageLoaded", ex);
                    }
                });
            }
        }

        private VideoStreamOptions CreateVideoStreamOptions(string itemId, UserItemDataDto userData, ref long ticks)
        {
            _itemId = itemId;

            if (userData != null && _isResume)
            {
                ticks = userData.PlaybackPositionTicks;
            }

            var query = new VideoStreamOptions
            {
                ItemId = itemId,
                VideoCodec = "H264",
                OutputFileExtension = ".mp4",
                AudioCodec = "Aac",
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

            return query;
        }
    }
}
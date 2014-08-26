using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using Microsoft.PlayerFramework;
using ScottIsAFool.WindowsPhone.ViewModel;
using System.Threading;
using System.Threading.Tasks;

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
        private long _startPositionTicks = 0;
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
                var totalTicks = _startPositionTicks + PlayedVideoDuration.Ticks;

                Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                var info = new PlaybackProgressInfo
                {
                    ItemId = _itemId,
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

        public PlayerSourceType PlayerSourceType { get; set; }
        public List<Caption> Captions { get; set; }

        public bool IsHls
        {
            get { return PlayerSourceType == PlayerSourceType.Programme; }
        }

        private void SetPlaybackTicks(long totalTicks)
        {
            switch (PlayerSourceType)
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

                PlayerSourceType = m.PlayerSourceType;
                _isResume = m.IsResume;
                _startPositionTicks = m.ResumeTicks.HasValue ? m.ResumeTicks.Value : 0;
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
                        var totalTicks = _startPositionTicks + PlayedVideoDuration.Ticks;

                        Log.Info("Sending current runtime [{0}] to the server", totalTicks);

                        var info = new PlaybackStopInfo
                        {
                            ItemId = _itemId,
                            //UserId = AuthenticationService.Current.LoggedInUserId,
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
                        var totalTicks = _startPositionTicks + PlayedVideoDuration.Ticks;
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
                            //UserId = AuthenticationService.Current.LoggedInUserId,
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
        public TimeSpan StartTime
        {
            get
            {
                return TimeSpan.FromTicks(_startPositionTicks * -1);
            }
        }
        private TimeSpan _endTime = TimeSpan.Zero;
        public TimeSpan EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;                
            }
        }

        public TimeSpan PlayedVideoDuration { get; set; }
        public BaseItemDto SelectedItem { get; set; }
        public RecordingInfoDto RecordingItem { get; set; }
        public ProgramInfoDto ProgrammeItem { get; set; }

        //Recover from tombestone
        public string ItemId { get; set; }
        public string ItemType { get; set; }
        public bool Recover { get; set; }

        public RelayCommand VideoPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!Recover)                       
                        await InitiatePlayback(_isResume);                    
                });
            }
        }

        private async Task InitiatePlayback(bool isResume)
        {
            EndTime = TimeSpan.Zero;

            var query = new VideoStreamOptions();
            switch (PlayerSourceType)
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

                    if (SelectedItem.UserData != null && isResume)
                    {
                        _startPositionTicks = SelectedItem.UserData.PlaybackPositionTicks;
                    }

                    query = CreateVideoStreamOptions(SelectedItem.Id, SelectedItem.UserData, _startPositionTicks);

                    if (SelectedItem.RunTimeTicks.HasValue)
                        EndTime = TimeSpan.FromTicks(SelectedItem.RunTimeTicks.Value - _startPositionTicks);

                    Log.Info("Playing {0} [{1}] ({2})", SelectedItem.Type, SelectedItem.Name, SelectedItem.Id);
                    break;
                case PlayerSourceType.Recording:
                    query = CreateVideoStreamOptions(RecordingItem.Id, RecordingItem.UserData, _startPositionTicks);

                    if (RecordingItem.RunTimeTicks.HasValue)
                        EndTime = TimeSpan.FromTicks(RecordingItem.RunTimeTicks.Value - _startPositionTicks);

                    Log.Info("Playing {0} [{1}] ({2})", RecordingItem.Type, RecordingItem.Name, RecordingItem.Id);
                    break;
                case PlayerSourceType.Programme:
                    query = CreateVideoStreamOptions(ProgrammeItem.ChannelId, ProgrammeItem.UserData, _startPositionTicks, true);

                    if (ProgrammeItem.RunTimeTicks.HasValue)
                        EndTime = TimeSpan.FromTicks(ProgrammeItem.RunTimeTicks.Value - _startPositionTicks);

                    Log.Info("Playing {0} [{1}] ({2})", ProgrammeItem.Type, ProgrammeItem.Name, ProgrammeItem.Id);
                    break;
            }

            var url = PlayerSourceType == PlayerSourceType.Programme ? _apiClient.GetHlsVideoStreamUrl(query) : _apiClient.GetVideoStreamUrl(query);
            Captions = GetSubtitles(SelectedItem);

            VideoUrl = url;
            Debug.WriteLine(VideoUrl);            

            Log.Debug(VideoUrl);

            try
            {
                Log.Info("Sending playback started message to the server.");

                var info = new PlaybackStartInfo
                {
                    ItemId = query.ItemId,
                    //UserId = AuthenticationService.Current.LoggedInUserId,
                    CanSeek = false,
                    QueueableMediaTypes = new List<string>()
                };

                await _apiClient.ReportPlaybackStartAsync(info);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("VideoPageLoaded", ex);
            }
        }

        public void StartUpdateTimer()
        {
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        private VideoStreamOptions CreateVideoStreamOptions(string itemId, UserItemDataDto userData, long startTimeTicks, bool useHls = false)
        {
            _itemId = itemId;

            var query = new VideoStreamOptions
            {
                ItemId = itemId,
                VideoCodec = "H264",
                OutputFileExtension = useHls ? ".m3u8" : ".mp4",
                AudioCodec = "Aac",
                VideoBitRate = WindowsPhone.Helpers.ResolutionHelper.DefaultVideoBitrate,
                AudioBitRate = 128000,
                MaxAudioChannels = 2,
                StartTimeTicks = startTimeTicks,
                Profile = "baseline",
                Level = "3",
                MaxWidth = WindowsPhone.Helpers.ResolutionHelper.Width // (int)bounds.Height
            };

            return query;
        }

        public async void Seek(long newPosition)
        {
            _timer.Stop();
            _startPositionTicks += newPosition;
            await InitiatePlayback(false);
        }
        public async void RecoverState()
        {            
            _isResume = true;

            var cts = new CancellationTokenSource(7000);
            try
            {
                if (ItemType == "Program")
                {
                    var program = await _apiClient.GetLiveTvProgramAsync(ItemId, AuthenticationService.Current.LoggedInUserId, cts.Token);
                    if (program == null || program.UserData == null)
                        return;
                    PlayerSourceType = PlayerSourceType.Programme;

                    ProgrammeItem = program;
                }
                else if (ItemType == "Recording") //TODO Verify this
                {
                    var recording = await _apiClient.GetLiveTvRecordingAsync(ItemId, AuthenticationService.Current.LoggedInUserId, cts.Token);
                    if (recording == null || recording.UserData == null)
                        return;
                    PlayerSourceType = PlayerSourceType.Recording;

                    RecordingItem = recording;
                }
                else
                {
                    var item = await _apiClient.GetItemAsync(ItemId, AuthenticationService.Current.LoggedInUserId);
                    if (item == null || item.UserData == null)
                        return;
                    PlayerSourceType = PlayerSourceType.Video;

                    SelectedItem = item;
                }
                await InitiatePlayback(true);
            }
            catch(TaskCanceledException ex)
            {
                Log.ErrorException("RecoverState timedout", ex);
            }            
            Recover = false;
                
        }

        internal List<Caption> GetSubtitles(BaseItemDto item)
        {
            var list = new List<Caption>();
            if (item.MediaStreams == null)
                return list;
            foreach (var caption in item.MediaStreams.Where(s => s.Type == MediaStreamType.Subtitle))
            {
                var menuItem = new Caption
                {
                    Id = caption.Index.ToString(CultureInfo.InvariantCulture),
                    //Language = caption.Language,
                    //Name = caption.Language,
                    Description = caption.Language
                };

                if (caption.IsTextSubtitleStream)
                {
                    var source = item.MediaSources.FirstOrDefault();
                    var id = source != null ? source.Id : string.Empty;
                    menuItem.Source = new Uri(string.Format("{0}/mediabrowser/Videos/{1}/{2}/Subtitles/{3}/Stream.vtt", _apiClient.ServerAddress, item.Id, id, caption.Index));
                }

                list.Add(menuItem);
            }
            return list;
        }
    }
}
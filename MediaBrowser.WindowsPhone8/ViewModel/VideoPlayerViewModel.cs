using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Dlna.Profiles;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using Microsoft.PlayerFramework;
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
        private readonly DispatcherTimer _timer;

        private bool _isResume;
        private long _startPositionTicks;
        private string _itemId;

        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(IConnectionManager connectionManager, INavigationService navigationService)
            : base(navigationService, connectionManager)
        {
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

                await ApiClient.ReportPlaybackProgressAsync(info);
                SetPlaybackTicks(totalTicks);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("TimerOnTick()", ex, NavigationService, Log);
            }
        }

        public PlayerSourceType PlayerSourceType { get; set; }
        public List<Caption> Captions { get; set; }

        public bool IsHls
        {
            get { return PlayerSourceType == PlayerSourceType.Programme || (SelectedItem != null && SelectedItem.Type.ToLower().Equals("channelvideoitem")); }
        }

        public IList<BaseItemDto> PlaylistItems { get; set; }

        public bool IsPlaylist
        {
            get { return PlaylistItems != null; }
        }

        public RelayCommand SkipNextCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!IsPlaylist || PlaylistItems.Count <= 1)
                    {
                        return;
                    }

                    var currentIndex = PlaylistItems.IndexOf(SelectedItem);
                    if (currentIndex < 0 || currentIndex == PlaylistItems.Count - 1)
                    {
                        return;
                    }

                    SelectedItem = PlaylistItems[currentIndex + 1];

                    InitiatePlayback(false).ConfigureAwait(false);
                });
            }
        }

        public RelayCommand SkipPreviousCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!IsPlaylist || PlaylistItems.Count <= 1)
                    {
                        return;
                    }

                    var currentIndex = PlaylistItems.IndexOf(SelectedItem);
                    if (currentIndex <= 0)
                    {
                        return;
                    }

                    SelectedItem = PlaylistItems[currentIndex - 1];
                    InitiatePlayback(false).ConfigureAwait(false);
                });
            }
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
                PlaylistItems = null;
                switch (m.PlayerSourceType)
                {
                    case PlayerSourceType.Video:
                        if (m.VideoItem != null)
                        {
                            SelectedItem = m.VideoItem;
                            PlaylistItems = null;
                        }
                        break;
                    case PlayerSourceType.Recording:
                        if (m.RecordingItem != null)
                        {
                            RecordingItem = m.RecordingItem;
                            PlaylistItems = null;
                        }
                        break;
                    case PlayerSourceType.Programme:
                        if (m.ProgrammeItem != null)
                        {
                            ProgrammeItem = m.ProgrammeItem;
                            PlaylistItems = null;
                        }
                        break;
                    case PlayerSourceType.Playlist:
                        PlaylistItems = m.VideoPlaylists;
                        SelectedItem = m.VideoItem;
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

                        await ApiClient.ReportPlaybackStoppedAsync(info);
                        await ApiClient.StopTranscodingProcesses(ApiClient.DeviceId);

                        SetPlaybackTicks(totalTicks);

                        if (_timer != null && _timer.IsEnabled)
                        {
                            _timer.Stop();
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("SendVideoTimeToServer", ex, NavigationService, Log);
                    }

                    if (!PlaylistItems.IsNullOrEmpty())
                    {
                        var index = PlaylistItems.IndexOf(SelectedItem);
                        if (index >= 0 && index < PlaylistItems.Count - 1)
                        {
                            SelectedItem = PlaylistItems[index + 1];
                            await InitiatePlayback(false);
                        }
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

                        await ApiClient.ReportPlaybackProgressAsync(info);
                        SetPlaybackTicks(totalTicks);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("VideoStateChanged", ex, NavigationService, Log);
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
                    if (App.SpecificSettings.OnlyStreamOnWifi)
                    {
                        if (!NavigationService.IsOnWifi)
                        {
                            MessageBox.Show(AppResources.ErrorNoWifi, AppResources.ErrorNoWifiTitle, MessageBoxButton.OK);
                            if (NavigationService.CanGoBack)
                            {
                                NavigationService.GoBack();
                            }
                            else
                            {
                                NavigationService.NavigateTo(Constants.Pages.MainPage);
                            }
                            return;
                        }
                    }

                    if (!Recover)                       
                        await InitiatePlayback(_isResume);                    
                });
            }
        }

        private async Task InitiatePlayback(bool isResume, int? subtitleIndex = null)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ClearNowPlayingMsg));
            EndTime = TimeSpan.Zero;

            var query = new StreamInfo();
            //var query = new VideoStreamOptions();
            switch (PlayerSourceType)
            {
                case PlayerSourceType.Playlist:
                case PlayerSourceType.Video:
                    if (SelectedItem.VideoType != VideoType.VideoFile)
                    {
                        var result = MessageBox.Show(AppResources.MessageExperimentalVideo, AppResources.MessageExperimentalTitle, MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.Cancel)
                        {
                            NavigationService.GoBack();
                            return;
                        }
                    }

                    if (SelectedItem.UserData != null && isResume)
                    {
                        _startPositionTicks = SelectedItem.UserData.PlaybackPositionTicks;
                    }

                    if (_startPositionTicks == 0)
                    {
                        try
                        {
                            var items = await ApiClient.GetIntrosAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId);
                            if (items != null && !items.Items.IsNullOrEmpty())
                            {
                                if (PlaylistItems == null)
                                {
                                    PlaylistItems = new List<BaseItemDto>(items.Items) {SelectedItem};
                                }
                                else
                                {
                                    var list = items.Items.ToList();
                                    list.AddRange(PlaylistItems);

                                    PlaylistItems = list;
                                }

                                var firstItem = PlaylistItems.FirstOrDefault();
                                if (firstItem != null) SelectedItem = firstItem;
                            }
                        }
                        catch (HttpException ex)
                        {
                            Log.ErrorException("GetIntros (Cinema Mode)", ex);
                        }
                    }

                    query = CreateVideoStream(SelectedItem.Id, _startPositionTicks, SelectedItem.MediaSources, SelectedItem.Type.ToLower().Equals("channelvideoitem"));
                    //query = CreateVideoStreamOptions(SelectedItem.Id, _startPositionTicks, SelectedItem.Type.ToLower().Equals("channelvideoitem"));

                    if (SelectedItem.RunTimeTicks.HasValue)
                        EndTime = TimeSpan.FromTicks(SelectedItem.RunTimeTicks.Value - _startPositionTicks);

                    Log.Info("Playing {0} [{1}] ({2})", SelectedItem.Type, SelectedItem.Name, SelectedItem.Id);
                    break;
                case PlayerSourceType.Recording:
                    //query = CreateVideoStreamOptions(RecordingItem.Id, _startPositionTicks);
                    query = CreateVideoStream(RecordingItem.Id, _startPositionTicks);

                    if (RecordingItem.RunTimeTicks.HasValue)
                        EndTime = TimeSpan.FromTicks(RecordingItem.RunTimeTicks.Value - _startPositionTicks);

                    Log.Info("Playing {0} [{1}] ({2})", RecordingItem.Type, RecordingItem.Name, RecordingItem.Id);
                    break;
                case PlayerSourceType.Programme:
                    //query = CreateVideoStreamOptions(ProgrammeItem.ChannelId, _startPositionTicks, true);
                    try
                    {
                        var channel = await ApiClient.GetItemAsync(ProgrammeItem.ChannelId, AuthenticationService.Current.LoggedInUserId);
                        query = CreateVideoStream(ProgrammeItem.ChannelId, _startPositionTicks, channel.MediaSources, useHls: true);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "GetVideoChannel", NavigationService, Log);
                        NavigationService.GoBack();
                        return;
                    }

                    if (ProgrammeItem.RunTimeTicks.HasValue)
                        EndTime = TimeSpan.FromTicks(ProgrammeItem.RunTimeTicks.Value - _startPositionTicks);

                    Log.Info("Playing {0} [{1}] ({2})", ProgrammeItem.Type, ProgrammeItem.Name, ProgrammeItem.Id);
                    break;
            }

            if (subtitleIndex.HasValue)
            {
                query.SubtitleStreamIndex = subtitleIndex.Value;
            }

            var url = query.ToUrl(ApiClient.GetApiUrl("/"));
            //var url = PlayerSourceType == PlayerSourceType.Programme ? ApiClient.GetHlsVideoStreamUrl(query) : ApiClient.GetVideoStreamUrl(query);
            //Captions = GetSubtitles(SelectedItem);

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

                await ApiClient.ReportPlaybackStartAsync(info);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("VideoPageLoaded", ex, NavigationService, Log);
            }
        }

        public void StartUpdateTimer()
        {
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        private VideoStreamOptions CreateVideoStreamOptions(string itemId, long startTimeTicks, bool useHls = false)
        {
            _itemId = itemId;

            var streamingSettings = NavigationService.IsOnWifi
                ? App.SpecificSettings.WifiStreamingQuality.GetSettings()
                : App.SpecificSettings.StreamingQuality.GetSettings();

            var query = new VideoStreamOptions
            {
                ItemId = itemId,
                VideoCodec = "H264",
                OutputFileExtension = useHls ? ".m3u8" : ".mp4",
                AudioCodec = "Aac",
                VideoBitRate = streamingSettings.VideoBitrate,
                AudioBitRate = streamingSettings.AudioBitrate,
                MaxAudioChannels = streamingSettings.AudioChannels,
                StartTimeTicks = startTimeTicks,
                Profile = "baseline",
                Level = "3",
                MaxWidth = streamingSettings.Width
            };

            return query;
        }

        private StreamInfo CreateVideoStream(string itemId, long startTimeTicks, List<MediaSourceInfo> mediaSources = null, bool useHls = false)
        {
            var profile = WindowsPhoneProfile.GetProfile(isHls: useHls);

            var streamingSettings = NavigationService.IsOnWifi
                ? App.SpecificSettings.WifiStreamingQuality.GetSettings()
                : App.SpecificSettings.StreamingQuality.GetSettings();

            var options = new VideoOptions
            {
                Profile = profile, 
                ItemId = itemId, 
                DeviceId = ApiClient.DeviceId, 
                MaxBitrate = streamingSettings.VideoBitrate,
                MediaSources = mediaSources
            };

            var builder = new StreamBuilder();
            var streamInfo = builder.BuildVideoItem(options);
            streamInfo.StartPositionTicks = startTimeTicks;

            return streamInfo;
        }

        public async void Seek(long newPosition, int? subtitleIndex = null)
        {
            _timer.Stop();
            _startPositionTicks += newPosition;
            await InitiatePlayback(false, subtitleIndex);
        }
        public async void RecoverState()
        {            
            _isResume = true;

            var cts = new CancellationTokenSource(7000);
            try
            {
                if (ItemType == "Program")
                {
                    var program = await ApiClient.GetLiveTvProgramAsync(ItemId, AuthenticationService.Current.LoggedInUserId, cts.Token);
                    if (program == null || program.UserData == null)
                        return;
                    PlayerSourceType = PlayerSourceType.Programme;

                    ProgrammeItem = program;
                }
                else if (ItemType == "Recording") //TODO Verify this
                {
                    var recording = await ApiClient.GetLiveTvRecordingAsync(ItemId, AuthenticationService.Current.LoggedInUserId, cts.Token);
                    if (recording == null || recording.UserData == null)
                        return;
                    PlayerSourceType = PlayerSourceType.Recording;

                    RecordingItem = recording;
                }
                else
                {
                    var item = await ApiClient.GetItemAsync(ItemId, AuthenticationService.Current.LoggedInUserId);
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
                    menuItem.Source = new Uri(string.Format("{0}/mediabrowser/Videos/{1}/{2}/Subtitles/{3}/Stream.vtt", ApiClient.ServerAddress, item.Id, id, caption.Index));
                }

                list.Add(menuItem);
            }
            return list;
        }
    }
}
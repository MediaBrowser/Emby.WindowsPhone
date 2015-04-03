using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cimbalino.Toolkit.Helpers;
using Cimbalino.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Users;
using MediaBrowser.WindowsPhone.Logging;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Connection;
using MediaBrowser.WindowsPhone.Model.Security;
using Microsoft.Phone.BackgroundAudio;
using ScottIsAFool.WindowsPhone.Logging;
using UserAction = Microsoft.Phone.BackgroundAudio.UserAction;

namespace MediaBrowser.WindowsPhone.AudioAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static ILog _logger;
        private static volatile bool _classInitialized;
        private PlaylistHelper _playlistHelper;
        private static IApplicationSettingsServiceHandler ApplicationSettings;
        private static IApiClient _apiClient;
        private static DispatcherTimer _dispatcherTimer;
        private static ILogger _mbLogger = new MBLogger(typeof(AudioPlayer));

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(async () =>
                {
                    await ConfigureThePlayer();

                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                    _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
                    _dispatcherTimer.Tick += DispatcherTimerOnTick;
                    _dispatcherTimer.Start();
                });
            }
        }

        private async Task ConfigureThePlayer()
        {
            var tcs = new TaskCompletionSource<bool>();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                WPLogger.AppVersion = ApplicationManifest.Current.App.Version;
                WPLogger.LogConfiguration.LogType = LogType.WriteToFile;
                WPLogger.LogConfiguration.LoggingIsEnabled = true;

                if (ApplicationSettings == null)
                {
                    ApplicationSettings = new ApplicationSettingsService().Legacy;
                }

                if (_logger == null)
                {
                    _logger = new WPLogger(GetType());
                }

                if (_playlistHelper == null)
                {
                    _playlistHelper = new PlaylistHelper(new StorageService());
                }

                if (_apiClient == null) CreateClient();

                tcs.SetResult(true);
            });

            await tcs.Task;
        }

        private static async void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (_apiClient == null || BackgroundAudioPlayer.Instance.PlayerState != PlayState.Playing)
            {
                return;
            }

            try
            {
                var track = BackgroundAudioPlayer.Instance.Track;
                var info = new PlaybackProgressInfo
                {
                    ItemId = track.Tag,
                    //UserId = _apiClient.CurrentUserId,
                    PositionTicks = BackgroundAudioPlayer.Instance.Position.Ticks,
                    IsMuted = false,
                    IsPaused = false
                };

                await _apiClient.ReportPlaybackProgressAsync(info);
            }
            catch (HttpException ex)
            {
                _logger.FatalException("DispatcherTimerOnTick()", ex);
            }
        }

        private static void CreateClient()
        {
            try
            {
                _logger.Info("Creating Client");
                var device = new Device { DeviceId = SharedUtils.GetDeviceId(), DeviceName = SharedUtils.GetDeviceName() };
                var server = ApplicationSettings.Get<ServerInfo>(Constants.Settings.DefaultServerConnection);
                var auth = ApplicationSettings.Get<AuthenticationResult>(Constants.Settings.AuthUserSetting);
                if (server == null || auth == null || auth.User == null)
                {
                    if (server == null) _logger.Info("No server information!");
                    if(auth == null) _logger.Info("No authentication info");
                    if(auth != null && auth.User == null) _logger.Info("No User info available");
                    return;
                }

                var serverAddress = server.LastConnectionMode.HasValue && server.LastConnectionMode.Value == ConnectionMode.Manual ? server.ManualAddress : server.RemoteAddress;
                var client = new ApiClient(_mbLogger, serverAddress, "Windows Phone 8", device, ApplicationManifest.Current.App.Version, new CryptographyProvider());
                client.SetAuthenticationInfo(auth.AccessToken, auth.User.Id);

                _apiClient = client;
            }
            catch (Exception ex)
            {
                _logger.FatalException("Error creating ApiClient", ex);
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            _logger.ErrorException("Player Error", e.ExceptionObject);
        }

        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override async void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            await ConfigureThePlayer();
            switch (playState)
            {
                case PlayState.TrackEnded:
                    _logger.Info("PlayStateChanged.TrackEnded");
                    player.Track = await GetNextTrack();
                    await InformOfPlayingTrack();
                    break;
                case PlayState.TrackReady:
                    _logger.Info("PlayStateChanged.TrackReady");
                    try
                    {
                        player.Play();
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("OnPlayStateChanged.TrackReady", ex);
                    }
                    NotifyComplete();
                    break;
                case PlayState.Shutdown:
                    await InformOfStoppedTrack();
                    break;
                case PlayState.Unknown:
                    _logger.Info("PlayStateChanged.Unknown");
                    NotifyComplete();
                    break;
                case PlayState.Stopped:
                    _logger.Info("PlayStateChanged.Stopped");
                    NotifyComplete();
                    //_playlistHelper.SetAllTracksToNotPlayingAndSave();
                    break;
                case PlayState.Paused:
                    _logger.Info("PlayStateChanged.Paused");
                    await _playlistHelper.SetAllTracksToNotPlayingAndSave();
                    await InformOfStoppedTrack();
                    break;
                default:
                    NotifyComplete();
                    break;
            }

        }

        private async Task InformOfStoppedTrack()
        {
            if (_apiClient == null)
            {
                return;
            }

            try
            {
                var track = BackgroundAudioPlayer.Instance.Track;
                if (track != null)
                {
                    var info = new PlaybackStopInfo
                    {
                        ItemId = track.Tag,
                        //UserId = _apiClient.CurrentUserId
                    };
                    await _apiClient.ReportPlaybackStoppedAsync(info);
                }
            }
            catch (HttpException ex)
            {
                _logger.FatalException("InformOfStoppedTrack()", ex);
            }

            NotifyComplete();
        }

        private async Task InformOfPlayingTrack()
        {
            if (_apiClient == null)
            {
                return;
            }

            try
            {
                var track = BackgroundAudioPlayer.Instance.Track;
                if (track != null)
                {
                    var info = new PlaybackStartInfo
                    {
                        CanSeek = false,
                        ItemId = track.Tag,
                        QueueableMediaTypes = new List<string>(),
                    };
                    await _apiClient.ReportPlaybackStartAsync(info);
                }
            }
            catch (HttpException ex)
            {
                _logger.FatalException("InformOfPlayingTrack()", ex);
            }

            NotifyComplete();
        }

        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override async void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            await ConfigureThePlayer();
            switch (action)
            {
                case UserAction.Play:
                    if (player.PlayerState != PlayState.Playing)
                    {
                        _logger.Info("OnUserAction.Play");
                        player.Play();
                    }
                    break;
                case UserAction.Stop:
                    _logger.Info("OnUserAction.Stop");
                    player.Stop();
                    break;
                case UserAction.Pause:
                    _logger.Info("OnUserAction.Pause");
                    player.Pause();
                    break;
                case UserAction.FastForward:
                    _logger.Info("OnUserAction.FastForward");
                    player.FastForward();
                    break;
                case UserAction.Rewind:
                    _logger.Info("OnUserAction.Rewind");
                    player.Rewind();
                    break;
                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;
                case UserAction.SkipNext:
                    _logger.Info("OnUserAction.SkipNext");
                    var nextTrack = await GetNextTrack();
                    if (nextTrack != null)
                    {
                        player.Track = nextTrack;
                    }
                    await InformOfPlayingTrack();
                    break;
                case UserAction.SkipPrevious:
                    _logger.Info("OnUserAction.SkipPrevious");
                    var previousTrack = await GetPreviousTrack();
                    if (previousTrack != null)
                    {
                        player.Track = previousTrack;
                    }
                    await InformOfPlayingTrack();
                    break;
            }

            NotifyComplete();
        }


        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private async Task<AudioTrack> GetNextTrack()
        {
            var playlist = await _playlistHelper.GetPlaylist();

            var items = playlist.PlaylistItems;

            if (items == null || !items.Any())
            {
                _logger.Info("GetNextTrack() : Playlist empty");
                return null;
            }

            _logger.Info("GetNextTrack() : Getting the current track");
            var currentTrack = items.FirstOrDefault(x => x.IsPlaying);

            PlaylistItem nextTrack;

            if (currentTrack == default(PlaylistItem))
            {
                _logger.Info("GetNextTrack() : Get first track from playlist");
                nextTrack = items.FirstOrDefault();
            }
            else
            {
                if (!playlist.IsOnRepeat && currentTrack.Id == items.Count && items.Count > 1)
                {
                    _logger.Info("GetNextTrack() : End of playlist");
                    return null;
                }

                _logger.Info("GetNextTrack() : Current track ID: " + currentTrack.Id + ", items in playlist: " + items.Count);
                nextTrack = currentTrack.Id == items.Count ? items.FirstOrDefault() : items[currentTrack.Id];
            }

            if (nextTrack == null)
            {
                _logger.Info("GetNextTrack() : No track to play");
                return null;
            }

            _logger.Info("GetNextTrack() : Next track ID: " + nextTrack.Id + ", items in playlist: " + items.Count);

            try
            {
                _logger.Info("GetNextTrack() : Getting the actual track details");
                var track = nextTrack.ToAudioTrack();

                _playlistHelper.SetAllTracksToNotPlaying(items);

                var item = items.FirstOrDefault(x => x.Id == nextTrack.Id);
                if (item != null)
                {
                    item.IsPlaying = true;
                }

                await _playlistHelper.SavePlaylist(playlist);

                // specify the track

                return track;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("GetNextTrack()", ex);
                return null;
            }
        }


        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private async Task<AudioTrack> GetPreviousTrack()
        {
            var playlist = await _playlistHelper.GetPlaylist();

            var items = playlist.PlaylistItems;

            if (items == null || !items.Any())
            {
                _logger.Info("GetPreviousTrack() : Playlist empty");
                return null;
            }

            _logger.Info("GetPreviousTrack() : Getting the current track");
            var currentTrack = items.FirstOrDefault(x => x.IsPlaying);

            PlaylistItem nextTrack;

            if (currentTrack == default(PlaylistItem))
            {
                _logger.Info("GetPreviousTrack() : Get first track from playlist");
                nextTrack = items.LastOrDefault();
            }
            else
            {
                if (!playlist.IsOnRepeat && currentTrack.Id == items.Count)
                {
                    _logger.Info("GetPreviousTrack() : End of playlist");
                    return null;
                }

                _logger.Info("GetPreviousTrack() : Current track ID: " + currentTrack.Id + ", items in playlist: " + items.Count);
                nextTrack = currentTrack.Id - 1 == 0 ? items.LastOrDefault() : (items[currentTrack.Id - 1]);
                _logger.Info("GetPreviousTrack() : Current track ID: " + currentTrack.Id + ", items in playlist: " + items.Count);
            }

            if (nextTrack == null)
            {
                _logger.Info("GetPreviousTrack() : No track to play");
                return null;
            }

            try
            {
                _logger.Info("GetPreviousTrack() : Getting the actual track details");
                var track = nextTrack.ToAudioTrack();

                _playlistHelper.SetAllTracksToNotPlaying(items);

                var item = items.FirstOrDefault(x => x.Id == nextTrack.Id);
                if (item != null)
                {
                    item.IsPlaying = true;
                }

                await _playlistHelper.SavePlaylist(playlist);

                // specify the track

                return track;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("GetPreviousTrack()", ex);
                return null;
            }
        }

        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                Abort();
            }
            else
            {
                NotifyComplete();
            }

        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {

        }
    }
}

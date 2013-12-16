using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using Microsoft.Phone.BackgroundAudio;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.AudioAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static ILog _logger;
        private static volatile bool _classInitialized;
        private readonly PlaylistHelper _playlistHelper;
        private static IExtendedApiClient _apiClient;
        private static DispatcherTimer _dispatcherTimer;
        
        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            _playlistHelper = new PlaylistHelper(new StorageService());
            _logger = new WPLogger(GetType());
            _apiClient = CreateClient();
            WPLogger.AppVersion = ApplicationManifest.Current.App.Version;
            WPLogger.LogConfiguration.LogType = LogType.WriteToFile;
            WPLogger.LogConfiguration.LoggingIsEnabled = true;

            _dispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
            _dispatcherTimer.Tick += DispatcherTimerOnTick;
            _dispatcherTimer.Start();

            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                });
            }
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
                await _apiClient.ReportPlaybackProgressAsync(track.Tag, _apiClient.CurrentUserId, BackgroundAudioPlayer.Instance.Position.Ticks, false, false);
            }
            catch (HttpException ex)
            {
                _logger.FatalException("DispatcherTimerOnTick()", ex);
            }
        }

        private static IExtendedApiClient CreateClient()
        {
            try
            {
                var applicationSettings = new ApplicationSettingsService();
                var connectionDetails = applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
                var client = new ExtendedApiClient(new NullLogger(), connectionDetails.HostName, int.Parse(connectionDetails.HostName), "Windows Phone", SharedUtils.GetDeviceName() + " Audio Player", SharedUtils.GetDeviceId(), ApplicationManifest.Current.App.Version);
                client.CurrentUserId = AuthenticationService.Current.LoggedInUserId;

                return client;
            }
            catch (Exception ex)
            {
                _logger.FatalException("Error creating ApiClient", ex);
                return null;
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
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackEnded:
                    _logger.Info("PlayStateChanged.TrackEnded");
                    player.Track = GetNextTrack();
                    break;
                case PlayState.TrackReady:
                    _logger.Info("PlayStateChanged.TrackReady");
                    player.Play();
                    break;
                case PlayState.Shutdown:
                    // TODO: Handle the shutdown state here (e.g. save state)
                    break;
                case PlayState.Unknown:
                    _logger.Info("PlayStateChanged.Unknown");
                    break;
                case PlayState.Stopped:
                    _logger.Info("PlayStateChanged.Stopped");
                    _playlistHelper.SetAllTracksToNotPlayingAndSave();
                    break;
                case PlayState.Paused:
                    _logger.Info("PlayStateChanged.Paused");
                    break;
                case PlayState.Playing:
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
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
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
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
                    player.Track = GetNextTrack();
                    break;
                case UserAction.SkipPrevious:
                    _logger.Info("OnUserAction.SkipPrevious");
                    AudioTrack previousTrack = GetPreviousTrack();
                    if (previousTrack != null)
                    {
                        player.Track = previousTrack;
                    }
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
        private AudioTrack GetNextTrack()
        {
            var playlist = _playlistHelper.GetPlaylist();

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
                if (!playlist.IsOnRepeat && currentTrack.Id == items.Count)
                {
                    _logger.Info("GetNextTrack() : End of playlist");
                    return null;
                }

                _logger.Info("GetNextTrack() : Current track ID: " + currentTrack.Id + ", items in playlist: " + items.Count);
                nextTrack = currentTrack.Id == items.Count ? items.FirstOrDefault() : items[currentTrack.Id];
                _logger.Info("GetNextTrack() : Current track ID: " + currentTrack.Id + ", items in playlist: " + items.Count);
            }

            if (nextTrack == null)
            {
                _logger.Info("GetNextTrack() : No track to play");
                return null;
            }

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

                _playlistHelper.SavePlaylist(playlist);

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
        private AudioTrack GetPreviousTrack()
        {
            var playlist = _playlistHelper.GetPlaylist();

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

                _playlistHelper.SavePlaylist(playlist);

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

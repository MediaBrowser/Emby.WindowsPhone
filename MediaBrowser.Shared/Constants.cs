namespace MediaBrowser.WindowsPhone
{
    public class Constants
    {
        public static string[] TileColours = { "D8502B", "009300", "A200A9", "0A58C1", "009FB0", "3290F5", "93009C", "AF1A3F" };

        public const string PhoneTileUrlFormat = "/Splashscreen.xaml?action={0}&id={1}&name={2}";

        public const int ImageQuality = 80;

        public const string PhotoUploadBackgroundTaskName = "MediaBrowser.WindowsPhone.PhotoUploadTask";
        
        public class Settings
        {
            public const string SelectedUserSetting = "UserSetting";
            public const string SelectedUserPinSetting = "SelectedUserPin";
            public const string ConnectionSettings = "ConnectionSettings";
            public const string SpecificSettings = "SpecificSettings";
            public const string ServerPluginInstalled = "ServerPluginInstalled";
            public const string IsRegistered = "IsRegistered";
            public const string UseNotifications = "UseNotifications";
            public const string DoNotShowFirstRun = "DoNotShowFirstRun";
            public const string PhotoUploadSettings = "PhotoUploadSettings";
            public const string ServerCredentialSettings = "ServerCredentialSettings.json";
            public const string DefaultServerConnection = "DefaultServerConnection";
            public const string AuthUserSetting = "SelectedUserSetting";
        }

        public class Messages
        {
            public const string FolderViewLoadedMsg = "FolderViewLoadedMsg";
            public const string MainPageLoadedMsg = "MainPageLoadedMsg";
            public const string ShowFolderMsg = "ShowFolderMsg";
            public const string ClearMovieCollectionMsg = "ClearMovieCollectionMsg";
            public const string LoadingPageLoadedMsg = "LoadingPageLoadedMsg";
            public const string ProfileViewLoadedMsg = "ProfileViewLoadedMsg";
            public const string DoLoginMsg = "DoLoginMsg";
            public const string ChangeProfileMsg = "ChangeProfileMsg";
            public const string ErrorLoggingInMsg = "ErrorLoggingInMsg";
            public const string ShowMovieMsg = "ShowMovieMsg";
            public const string MovieViewLoadedMsg = "MovieViewLoadedMsg";
            public const string TvShowPageLoadedMsg = "TvShowPageLoadedMsg";
            public const string TvSeasonPageLoadedMsg = "TvSeasonPageLoadedMsg";
            public const string TvEpisodePageLoadedMsg = "TvEpisodePageLoadedMsg";
            public const string TvAllSeasonsPageLoadedMsg = "TvAllSeasonsPageLoadedMsg";
            public const string TvShowSelectedMsg = "TvSeriesSelectedMsg";
            public const string GetTvInformationMsg = "GetTvInformationMsg";
            public const string MovieViewBackMsg = "MovieViewBackMsg";
            public const string FolderViewBackMsg = "FolderViewBackMsg";

            public const string ChangeGroupingMsg = "ChangeGroupingMsg";
            public const string ClearFoldersMsg = "ClearFoldersMsg";
            public const string ShowTvSeries = "ShowTvSeries";
            public const string ClearFilmAndTvMsg = "ClearFilmAndTVMsg";
            public const string ShowSeasonMsg = "ShowSeasonMsg";
            public const string ClearEpisodesMsg = "ClearEpisodesMsg";
            public const string ShowEpisodeMsg = "ShowEpisodeMsg";
            public const string SplashAnimationFinishedMsg = "SplashAnimationFinishedMsg";
            public const string ClearTvSeriesMsg = "ClearTvSeriesMsg";
            public const string TvEpisodeSelectedMsg = "TvEpisodeSelectedMsg";
            public const string TvSeasonSelectedMsg = "TvSeasonSelectedMsg";
            public const string ClearEverythingMsg = "ClearEverythingMsg";
            public const string VideoPlayerLoadedMsg = "VideoPlayerLoadedMsg";
            public const string SendVideoTimeToServerMsg = "SendVideoTimeToServerMsg";
            public const string PlayTrailerMsg = "PlayTrailerMsg";
            public const string ChangeTrailerMsg = "ChangeTrailerMsg";
            public const string TrailerPageLoadedMsg = "TrailerPageLoadedMsg";
            public const string MusicArtistChangedMsg = "MusicArtistChangedMsg";
            public const string ArtistViewLoadedMsg = "ArtistViewLoadedMsg";
            public const string MusicAlbumChangedMsg = "MusicAlbumChangedMsg";
            public const string AlbumViewLoadedMsg = "AlbumViewLoadedMsg";
            public const string CheckForPushPluginMsg = "CheckForPushPluginMsg";
            public const string NotificationSettingsLoadedMsg = "NotificationSettingsLoadedMsg";
            public const string CollectionPinnedMsg = "CollectionPinnedMsg";
            public const string ResetAppMsg = "ResetAppMsg";
            public const string LeftTrailerMsg = "LeftTrailerMsg";
            public const string NoResultsMsg = "NoResultsMsg";
            public const string SearchPageLoadedMsg = "SearchPageLoadedMsg";
            public const string ChangeFilteredResultsMsg = "ChangeFilteredResultsMsg";
            public const string SendTracksToNowPlayingMsg = "SendTracksToNowPlayingMsg";
            public const string CurrentPlaylist = "CurrentPlaylist";
            public const string AddToPlaylistMsg = "AddToPlaylistMsg";
            public const string SetPlaylistAsMsg = "SetPlaylistAsMsg";
            public const string PlaylistPageLeftMsg = "PlaylistPageLeftMsg";
            public const string NotificationCountMsg = "NotificationCountMsg";
            public const string NotifcationNavigationMsg = "NotifcationNavigationMsg";
            public const string ChangeActorMsg = "ChangeActorMsg";
            public const string GenericItemChangedMsg = "GenericItemChangedMsg";
            public const string VideoStateChangedMsg = "VideoStateChangedMsg";
            public const string SetResumeMsg = "SetResumeMsg";
            public const string ChangeChannelMsg = "ChangeChannelMsg";
            public const string ScheduledSeriesChangedMsg = "ScheduledSeriesChangedMsg";
            public const string ScheduledSeriesCancelChangesMsg = "ScheduledSeriesCancelChangesMsg";
            public const string ScheduledRecordingChangedMsg = "ScheduledRecordingChangedMsg";
            public const string ShowAllProgrammesMsg = "ShowAllProgrammesMsg";
            public const string ProgrammeItemChangedMsg = "ProgrammeItemChangedMsg";
            public const string LiveTvSeriesDeletedMsg = "LiveTvSeriesDeletedMsg";
            public const string NewSeriesRecordingAddedMsg = "NewSeriesRecordingAddedMsg";
            public const string ChangeRecordingGroupingMsg = "ChangeRecordingGroupingMsg";
            public const string ClearNowPlayingMsg = "ClearNowPlayingMsg";
            public const string ServerPlaylistChangedMsg = "ServerPlaylistChangedMsg";
            public const string AddToServerPlaylistsMsg = "AddToServerPlaylistsMsg";
        }

        public class Pages
        {
            private const string ViewsPath = "/Views/";
            public const string HomePage = ViewsPath + "MainPage.xaml";
            public const string ChooseProfileView = ViewsPath + "ChooseProfileView.xaml";
            public const string ManualUsernameView = ViewsPath + "ManualUsernameView.xaml";
            public const string SettingsView = ViewsPath + "SettingsView.xaml";
            public const string SettingsViewConnection = ViewsPath + "Settings/ConnectionSettingsView.xaml";
            public const string NotificationsView = ViewsPath + "NotificationsView.xaml";
            public const string NotificationView = ViewsPath + "NotificationView.xaml";
            public const string FolderView = ViewsPath + "FolderView.xaml?id=";
            public const string CollectionView = ViewsPath + "CollectionView.xaml";
            public const string MovieView = ViewsPath + "MovieView.xaml";
            public const string TvShowView = ViewsPath + "TvShowView.xaml";
            public const string SeasonView = ViewsPath + "SeasonView.xaml";
            public const string EpisodeView = ViewsPath + "EpisodeView.xaml";
            public const string TrailerView = ViewsPath + "TrailerView.xaml";
            public const string ArtistView = ViewsPath + "ArtistView.xaml";
            public const string AlbumView = ViewsPath + "AlbumView.xaml";
            public const string NowPlayingView = ViewsPath + "NowPlayingView.xaml";
            public const string FullPlaylistView = ViewsPath + "FullPlaylistView.xaml";
            public const string SearchView = ViewsPath + "SearchView.xaml";
            public const string VideoPlayerView = ViewsPath + "VideoPlayerView.xaml?id={0}&type={1}";
            public const string ActorView = ViewsPath + "ActorView.xaml";
            public const string GenericItemView = ViewsPath + "GenericItemView.xaml";
            public const string MainPage = ViewsPath + "MainPage.xaml";
            public const string SplashScreen = "/Splashscreen.xaml";

            public class SettingsViews
            {
                private const string SettingsViewPath = ViewsPath + "Settings/";
                public const string ConnectionSettingsView = SettingsViewPath + "ConnectionSettingsView.xaml";
                public const string GeneralSettingsView = SettingsViewPath + "GeneralSettingsView.xaml";
                public const string LiveTvSettingsView = SettingsViewPath + "LiveTvSettingsView.xaml";
                public const string LockScreenSettingsView = SettingsViewPath + "LockScreenSettingsView.xaml";
                public const string PhotoUploadSettingsView = SettingsViewPath + "PhotoUploadSettingsView.xaml";
                public const string StreamingSettingsView = SettingsViewPath + "StreamingSettingsView.xaml";
                public const string TileSettingsView = SettingsViewPath + "TileSettingsView.xaml";
                public const string FindServerView = SettingsViewPath + "FindServerView.xaml";
                public const string MbConnectView = SettingsViewPath + "MbConnectView.xaml";
            }

            public class LiveTv
            {
                private const string LiveTvPath = ViewsPath + "LiveTv/";
                public const string ChannelsView = LiveTvPath + "ChannelsView.xaml";
                public const string GuideView = LiveTvPath + "GuideView.xaml";
                public const string ScheduleView = LiveTvPath + "ScheduleView.xaml";
                public const string LiveTvView = LiveTvPath + "LiveTvView.xaml";
                public const string ScheduledSeriesView = LiveTvPath + "ScheduledSeriesView.xaml";
                public const string ScheduledRecordingView = LiveTvPath + "ScheduledRecordingView.xaml";
                public const string ProgrammeView = LiveTvPath + "ProgrammeView.xaml";
                public const string AllProgrammesView = LiveTvPath + "AllProgrammesView.xaml";
                public const string RecordedTvView = LiveTvPath + "RecordedTvView.xaml";
                public const string RecordingView = LiveTvPath + "RecordingView.xaml";
            }

            public class Playlists
            {
                private const string PlaylistPath = ViewsPath + "Playlists/";
                public const string PlaylistView = PlaylistPath + "PlaylistView.xaml";
                public const string AddToPlaylistView = PlaylistPath + "AddToPlaylistView.xaml";
            }

            public class Remote
            {
                private const string RemotePath = ViewsPath + "Remote/";
                public const string RemoteView = RemotePath + "RemoteView.xaml";
                public const string ChooseClientView = RemotePath + "ChooseClientView.xaml";
            }

            public class Predefined
            {
                private const string PredefinedPath = ViewsPath + "Predefined/";
                public const string MusicCollectionView = PredefinedPath + "MusicCollectionView.xaml";
                public const string TvCollectionView = PredefinedPath + "TvCollectionView.xaml";
                public const string MovieCollectionView = PredefinedPath + "MovieCollectionView.xaml";
            }

            public class FirstRun
            {
                private const string FirstRunPath = ViewsPath + "FirstRun/";
                public const string WelcomeView = FirstRunPath + "WelcomeView.xaml";
                public const string ConfigureView = FirstRunPath + "ConfigureView.xaml";
                public const string MbConnectFirstRunView = FirstRunPath + "MbConnectFirstRunView.xaml";
            }

            public class Channels
            {
                private const string ChannelsPath = ViewsPath + "Channels/";
                public const string ChannelsView = ChannelsPath + "ChannelsView.xaml";
                public const string ChannelView = ChannelsPath + "ChannelView.xaml?id=";
            }
        }
    }
}

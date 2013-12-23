namespace MediaBrowser
{
    public class Constants
    {
        public static string[] TileColours = { "D8502B", "009300", "A200A9", "0A58C1", "009FB0", "3290F5", "93009C", "AF1A3F" };

        public static string PhoneTileUrlFormat = "/Splashscreen.xaml?action={0}&id={1}&name={2}";
        
        public class Settings
        {
            public const string SelectedUserSetting = "SelectedUser";
            public const string SelectedUserPinSetting = "SelectedUserPin";
            public const string ConnectionSettings = "ConnectionSettings";
            public const string SpecificSettings = "SpecificSettings";
            public const string ServerPluginInstalled = "ServerPluginInstalled";
            public const string IsRegistered = "IsRegistered";
            public const string UseNotifications = "UseNotifications";
            public const string DoNotShowFirstRun = "DoNotShowFirstRun";
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
            public const string PlayVideoItemMsg = "PlayVideoItemMsg";
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
            public const string HideChapterWindowMsg = "HideChapterWindowMsg";
        }

        public class Pages
        {
            private const string ViewsPath = "/Views/";
            public const string HomePage = ViewsPath + "MainPage.xaml";
            public const string ChooseProfileView = ViewsPath + "ChooseProfileView.xaml";
            public const string ManualUsernameView = ViewsPath + "ManualUsernameView.xaml";
            public const string SettingsView = ViewsPath + "SettingsView.xaml";
            public const string SettingsViewConnection = ViewsPath + "SettingsView.xaml?settingsPane=1";
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
            public const string VideoPlayerView = ViewsPath + "VideoPlayerView.xaml";
            public const string ActorView = ViewsPath + "ActorView.xaml";
            public const string GenericItemView = ViewsPath + "GenericItemView.xaml";
            public const string MainPage = ViewsPath + "MainPage.xaml";

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
            }
        }
    }
}

using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction.Cryptography;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.WindowsPhone.Design;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Interfaces;
using MediaBrowser.WindowsPhone.Logging;
using MediaBrowser.WindowsPhone.Model.Security;
using MediaBrowser.WindowsPhone.Model.Sync;
using MediaBrowser.WindowsPhone.Services;
using MediaBrowser.WindowsPhone.ViewModel.Channels;
using MediaBrowser.WindowsPhone.ViewModel.Playlists;
using MediaBrowser.WindowsPhone.ViewModel.Predefined;
using MediaBrowser.WindowsPhone.ViewModel.Remote;
using MediaBrowser.WindowsPhone.ViewModel.Settings;
using Microsoft.Practices.ServiceLocation;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;
using NavigationService = MediaBrowser.WindowsPhone.Services.NavigationService;
using MediaBrowser.WindowsPhone.ViewModel.LiveTv;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var device = new Device
            {
                DeviceName = SharedUtils.GetDeviceName(),
                DeviceId = SharedUtils.GetDeviceId()
            };

            var logger = new MBLogger();

            SimpleIoc.Default.RegisterIf<ILogger>(() => logger);
            SimpleIoc.Default.RegisterIf<IDevice>(() => device);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.RegisterIf<INavigationService, NavigationService>();
                SimpleIoc.Default.RegisterIf<FolderViewModel>();
                SimpleIoc.Default.RegisterIf<MovieViewModel>();
                SimpleIoc.Default.RegisterIf<IApplicationSettingsService, ApplicationSettingsDesignService>();
                SimpleIoc.Default.RegisterIf<IStorageService, StorageDesignService>();
            }
            else
            {
                SimpleIoc.Default.RegisterIf<INavigationService, NavigationService>();
                SimpleIoc.Default.RegisterIf<ISettingsService, SettingsService>();
                SimpleIoc.Default.RegisterIf(() => SharedUtils.CreateConnectionManager(device, logger));
                SimpleIoc.Default.RegisterIf<IApplicationSettingsService, ApplicationSettingsService>();
                SimpleIoc.Default.RegisterIf<IStorageService, StorageService>();
                SimpleIoc.Default.RegisterIf<AuthenticationService>(true);
                SimpleIoc.Default.RegisterIf<LockScreenService>(true);
                SimpleIoc.Default.RegisterIf<TileService>(true);
                SimpleIoc.Default.RegisterIf<SyncService>(true);

                AddSyncInterfaces();

                SimpleIoc.Default.RegisterIf<PlaybackManager>();
            }

            SimpleIoc.Default.RegisterIf<IMessageBoxService, MessageBoxService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<VideoPlayerViewModel>();
            SimpleIoc.Default.Register<SplashscreenViewModel>();
            SimpleIoc.Default.Register<ChooseProfileViewModel>();
            SimpleIoc.Default.Register<TvViewModel>();
            SimpleIoc.Default.Register<TrailerViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<MusicViewModel>();
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<NowPlayingViewModel>(true);
            SimpleIoc.Default.Register<NotificationsViewModel>();
            SimpleIoc.Default.Register<RemoteViewModel>();
            SimpleIoc.Default.Register<MovieCollectionViewModel>();
            SimpleIoc.Default.Register<TvCollectionViewModel>();
            SimpleIoc.Default.Register<MusicCollectionViewModel>();
            SimpleIoc.Default.Register<ActorViewModel>();
            SimpleIoc.Default.Register<GenericItemViewModel>();
            SimpleIoc.Default.Register<LiveTvChannelsViewModel>();
            SimpleIoc.Default.Register<GuideViewModel>();
            SimpleIoc.Default.Register<ScheduleViewModel>();
            SimpleIoc.Default.Register<LiveTvViewModel>();
            SimpleIoc.Default.Register<ScheduledSeriesViewModel>();
            SimpleIoc.Default.Register<ScheduledRecordingViewModel>();
            SimpleIoc.Default.Register<AllProgrammesViewModel>();
            SimpleIoc.Default.Register<ProgrammeViewModel>();
            SimpleIoc.Default.Register<RecordedTvViewModel>();
            SimpleIoc.Default.Register<ChannelViewModel>();
            SimpleIoc.Default.Register<ChannelsViewModel>();
            SimpleIoc.Default.Register<ServerPlaylistsViewModel>();
            SimpleIoc.Default.Register<AddToPlaylistViewModel>();
            SimpleIoc.Default.Register<PhotoUploadViewModel>();
            SimpleIoc.Default.Register<MbConnectViewModel>();
        }

        private static void AddSyncInterfaces()
        {
            SimpleIoc.Default.RegisterIf<IUserActionRepository, UserActionRepository>();
            SimpleIoc.Default.RegisterIf<IItemRepository, ItemRepository>();
            SimpleIoc.Default.RegisterIf<IFileRepository, FileRepository>();
            SimpleIoc.Default.RegisterIf<ICryptographyProvider, CryptographyProvider>();
            SimpleIoc.Default.RegisterIf<IUserRepository, UserRepository>();
            SimpleIoc.Default.RegisterIf<IImageRepository, ImageRepository>();
            SimpleIoc.Default.RegisterIf<ILocalAssetManager, LocalAssetManager>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FolderViewModel Folder
        {
            get { return ServiceLocator.Current.GetInstance<FolderViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MovieViewModel Movie
        {
            get { return ServiceLocator.Current.GetInstance<MovieViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TvViewModel Tv
        {
            get { return ServiceLocator.Current.GetInstance<TvViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SplashscreenViewModel Splashscreen
        {
            get { return ServiceLocator.Current.GetInstance<SplashscreenViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChooseProfileViewModel Profile
        {
            get { return ServiceLocator.Current.GetInstance<ChooseProfileViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public VideoPlayerViewModel Player
        {
            get { return ServiceLocator.Current.GetInstance<VideoPlayerViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TrailerViewModel Trailer
        {
            get { return ServiceLocator.Current.GetInstance<TrailerViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MusicViewModel Music
        {
            get { return ServiceLocator.Current.GetInstance<MusicViewModel>(); }
        }

        public SearchViewModel Search
        {
            get { return ServiceLocator.Current.GetInstance<SearchViewModel>(); }
        }

        public NowPlayingViewModel NowPlaying
        {
            get { return ServiceLocator.Current.GetInstance<NowPlayingViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public NotificationsViewModel Notifications
        {
            get { return ServiceLocator.Current.GetInstance<NotificationsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MovieCollectionViewModel MovieCollection
        {
            get { return ServiceLocator.Current.GetInstance<MovieCollectionViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MusicCollectionViewModel MusicCollection
        {
            get { return ServiceLocator.Current.GetInstance<MusicCollectionViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TvCollectionViewModel TvCollection
        {
            get { return ServiceLocator.Current.GetInstance<TvCollectionViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RemoteViewModel Remote
        {
            get { return ServiceLocator.Current.GetInstance<RemoteViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ActorViewModel Actor
        {
            get { return ServiceLocator.Current.GetInstance<ActorViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GenericItemViewModel Generic
        {
            get { return ServiceLocator.Current.GetInstance<GenericItemViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LiveTvChannelsViewModel LiveTvChannels
        {
            get { return ServiceLocator.Current.GetInstance<LiveTvChannelsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GuideViewModel Guide
        {
            get { return ServiceLocator.Current.GetInstance<GuideViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ScheduleViewModel Schedule
        {
            get { return ServiceLocator.Current.GetInstance<ScheduleViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LiveTvViewModel LiveTv
        {
            get { return ServiceLocator.Current.GetInstance<LiveTvViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ScheduledSeriesViewModel ScheduledSeries
        {
            get { return ServiceLocator.Current.GetInstance<ScheduledSeriesViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ScheduledRecordingViewModel ScheduledRecording
        {
            get { return ServiceLocator.Current.GetInstance<ScheduledRecordingViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AllProgrammesViewModel AllProgrammes
        {
            get { return ServiceLocator.Current.GetInstance<AllProgrammesViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ProgrammeViewModel Programme
        {
            get { return ServiceLocator.Current.GetInstance<ProgrammeViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RecordedTvViewModel RecordedTv
        {
            get { return ServiceLocator.Current.GetInstance<RecordedTvViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelsViewModel Channels
        {
            get { return ServiceLocator.Current.GetInstance<ChannelsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelViewModel Channel
        {
            get { return ServiceLocator.Current.GetInstance<ChannelViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ServerPlaylistsViewModel ServerPlaylists
        {
            get { return ServiceLocator.Current.GetInstance<ServerPlaylistsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public AddToPlaylistViewModel AddToPlaylist
        {
            get { return ServiceLocator.Current.GetInstance<AddToPlaylistViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public PhotoUploadViewModel PhotoUpload
        {
            get { return ServiceLocator.Current.GetInstance<PhotoUploadViewModel>(); }
        }

        public MbConnectViewModel MbConnect
        {
            get { return ServiceLocator.Current.GetInstance<MbConnectViewModel>(); }
        }

        public static TvViewModel GetTvViewModel(string itemId)
        {
            return ServiceLocator.Current.GetInstance<TvViewModel>(itemId);
        }

        public static ChannelViewModel GetChannelViewModel(string itemId)
        {
            return ServiceLocator.Current.GetInstance<ChannelViewModel>(itemId);
        }

        public static IConnectionManager ConnectionManager
        {
            get { return ServiceLocator.Current.GetInstance<IConnectionManager>(); }
        }

        public static INavigationService NavigationService
        {
            get { return ServiceLocator.Current.GetInstance<INavigationService>(); }
        }

        public static AuthenticationService Auth
        {
            get { return ServiceLocator.Current.GetInstance<AuthenticationService>(); }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            foreach (var vm in ServiceLocator.Current.GetAllInstances<ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase>())
            {
                vm.Cleanup();
            }
        }
    }
}
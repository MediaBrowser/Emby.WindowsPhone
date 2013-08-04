/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MediaBrowser.WindowsPhone.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Design;
using MediaBrowser.WindowsPhone.ViewModel.Predefined;
using Microsoft.Practices.ServiceLocation;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;
using NavigationService = MediaBrowser.WindowsPhone.Model.NavigationService;
using MediaBrowser.ApiInteraction;
using Cimbalino.Phone.Toolkit.Helpers;

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

            if (ViewModelBase.IsInDesignModeStatic)
            {
                if (!SimpleIoc.Default.IsRegistered<IExtendedApiClient>())
                    SimpleIoc.Default.Register<IExtendedApiClient, ExtendedApiClientDesign>();
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
                SimpleIoc.Default.Register<FolderViewModel>();
                SimpleIoc.Default.Register<MovieViewModel>();
                if(!SimpleIoc.Default.IsRegistered<IApplicationSettingsService>())
                    SimpleIoc.Default.Register<IApplicationSettingsService, ApplicationSettingsDesignService>();

                if(!SimpleIoc.Default.IsRegistered<IStorageService>())
                    SimpleIoc.Default.Register<IStorageService, StorageDesignService>();
            }
            else
            {
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
                SimpleIoc.Default.Register<ISettingsService, SettingsService>();
                if (!SimpleIoc.Default.IsRegistered<IExtendedApiClient>())
                    SimpleIoc.Default.Register<IExtendedApiClient>(() => new ExtendedApiClient(new MBLogger(), "dummy", 8096, "Windows Phone", Utils.GetDeviceName(), Utils.GetDeviceId(), ApplicationManifest.Current.App.Version));

                if (!SimpleIoc.Default.IsRegistered<IDeviceExtendedPropertiesService>())
                    SimpleIoc.Default.Register<IDeviceExtendedPropertiesService, DeviceExtendedPropertiesService>();

                if (!SimpleIoc.Default.IsRegistered<IUserExtendedPropertiesService>())
                    SimpleIoc.Default.Register<IUserExtendedPropertiesService, UserExtendedPropertiesService>();
                
                if (!SimpleIoc.Default.IsRegistered<IApplicationSettingsService>())
                    SimpleIoc.Default.Register<IApplicationSettingsService, ApplicationSettingsService>();

                if(!SimpleIoc.Default.IsRegistered<IStorageService>())
                    SimpleIoc.Default.Register<IStorageService, StorageService>();
            }

            SimpleIoc.Default.Register<MainViewModel>(true);
            SimpleIoc.Default.Register<VideoPlayerViewModel>(true);
            SimpleIoc.Default.Register<SplashscreenViewModel>();
            SimpleIoc.Default.Register<ChooseProfileViewModel>();
            SimpleIoc.Default.Register<TvViewModel>();
            SimpleIoc.Default.Register<TrailerViewModel>(true);
            SimpleIoc.Default.Register<SettingsViewModel>(true);
            SimpleIoc.Default.Register<MusicViewModel>(true);
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<PlaylistViewModel>(true);
            SimpleIoc.Default.Register<NotificationsViewModel>();
            SimpleIoc.Default.Register<RemoteViewModel>();
            SimpleIoc.Default.Register<MovieCollectionViewModel>();
            SimpleIoc.Default.Register<TvCollectionViewModel>();
            SimpleIoc.Default.Register<MusicCollectionViewModel>();
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

        public PlaylistViewModel Playlist
        {
            get { return ServiceLocator.Current.GetInstance<PlaylistViewModel>(); }
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

        public static TvViewModel GetTvViewModel(string itemId)
        {
            return ServiceLocator.Current.GetInstance<TvViewModel>(itemId);
        }

        public static IExtendedApiClient ApiClient
        {
            get { return ServiceLocator.Current.GetInstance<IExtendedApiClient>(); }
        }

        public static INavigationService NavigationService
        {
            get { return ServiceLocator.Current.GetInstance<INavigationService>(); }
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
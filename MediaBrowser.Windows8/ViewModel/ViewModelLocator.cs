/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:MediaBrowser.Windows8"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.Net;
using System.Net.Http;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Connectivity;
using MediaBrowser.Shared;
using MediaBrowser.Windows8.Model;
using Microsoft.Practices.ServiceLocation;
using Windows.UI.Xaml.Controls;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                
                if (!SimpleIoc.Default.IsRegistered<NavigationService>())
                    SimpleIoc.Default.Register(
                        () => new NavigationService(new Frame()));

                if (!SimpleIoc.Default.IsRegistered<ExtendedApiClient>())
                    SimpleIoc.Default.Register(() => new ExtendedApiClient(new Logger(), new AsyncHttpClient())
                                                                    {
                                                                        ServerHostName = "192.168.0.2",
                                                                        ServerApiPort = 8096,
                                                                        ClientType = ClientType.WindowsRT
                                                                    });
                if (!SimpleIoc.Default.IsRegistered<FolderViewModel>())
                    SimpleIoc.Default.Register<FolderViewModel>();
                if (!SimpleIoc.Default.IsRegistered<TvViewModel>())
                    SimpleIoc.Default.Register<TvViewModel>();
                if (!SimpleIoc.Default.IsRegistered<MovieViewModel>())
                    SimpleIoc.Default.Register<MovieViewModel>(true);
            }
            else
            {
                SimpleIoc.Default.Register<NavigationService>();
                if (!SimpleIoc.Default.IsRegistered<ExtendedApiClient>())
                    SimpleIoc.Default.Register(() => new ExtendedApiClient(new Logger(), new AsyncHttpClient(new HttpClientHandler
                                                                                                                 {
                                                                                                                     AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                                                                                                                 }))
                                                         {
                                                             ClientType = ClientType.WindowsRT,
                                                         });
                
            }

            if (!SimpleIoc.Default.IsRegistered<SpecificSettings>())
                SimpleIoc.Default.Register<SpecificSettings>(true);
            SimpleIoc.Default.Register<LoadingViewModel>(true);
            SimpleIoc.Default.Register<SelectProfilesViewModel>(true);
            SimpleIoc.Default.Register<MainViewModel>(true);
            SimpleIoc.Default.Register<VideoPlayerViewModel>(true);
            SimpleIoc.Default.Register<TvViewModel>();
            SimpleIoc.Default.Register<TrailerViewModel>(true);
            SimpleIoc.Default.Register<MusicViewModel>(true);
            SimpleIoc.Default.Register<NotificationsViewModel>(true);

        }

        public LoadingViewModel Loading
        {
            get { return ServiceLocator.Current.GetInstance<LoadingViewModel>(); }
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public FolderViewModel Folder
        {
            get { return ServiceLocator.Current.GetInstance<FolderViewModel>(); }
        }

        public SelectProfilesViewModel Profiles
        {
            get { return ServiceLocator.Current.GetInstance<SelectProfilesViewModel>(); }
        }

        public MovieViewModel Movie
        {
            get { return ServiceLocator.Current.GetInstance<MovieViewModel>(); }
        }

        public TvViewModel Tv
        {
            get { return ServiceLocator.Current.GetInstance<TvViewModel>(); }
        }

        public VideoPlayerViewModel Video
        {
            get { return ServiceLocator.Current.GetInstance<VideoPlayerViewModel>(); }
        }

        public TrailerViewModel Trailer
        {
            get { return ServiceLocator.Current.GetInstance<TrailerViewModel>(); }
        }

        public MusicViewModel Music
        {
            get { return ServiceLocator.Current.GetInstance<MusicViewModel>(); }
        }

        public NotificationsViewModel Notifications
        {
            get { return ServiceLocator.Current.GetInstance<NotificationsViewModel>(); }
        }

        public SpecificSettings SpecificSettings
        {
            get { return ServiceLocator.Current.GetInstance<SpecificSettings>(); }
        }

        public static TvViewModel GetTvViewModel(string itemId)
        {
            return ServiceLocator.Current.GetInstance<TvViewModel>(itemId);
        }

        public static ExtendedApiClient ApiClient
        {
            get { return ServiceLocator.Current.GetInstance<ExtendedApiClient>(); }
        }

        public static NavigationService NavigationService
        {
            get { return ServiceLocator.Current.GetInstance<NavigationService>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
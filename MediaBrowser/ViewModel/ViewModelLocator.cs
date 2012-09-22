/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MediaBrowser.WindowsPhone.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction.WindowsPhone;
using Microsoft.Practices.ServiceLocation;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model;

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
                SimpleIoc.Default.Register<ApiClient>();
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
            }
            else
            {
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
                SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            }

            SimpleIoc.Default.Register<MainViewModel>(true);
            SimpleIoc.Default.Register<FolderViewModel>(true);      
            SimpleIoc.Default.Register<MovieViewModel>(true);
            SimpleIoc.Default.Register<TvViewModel>(true);
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

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<FolderViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<MovieViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<TvViewModel>().Cleanup();
        }
    }
}
using GalaSoft.MvvmLight;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RemoteViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ExtendedApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the RemoteViewModel class.
        /// </summary>
        public RemoteViewModel(INavigationService navigationService, ExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }
    }
}
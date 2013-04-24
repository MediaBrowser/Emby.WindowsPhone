using GalaSoft.MvvmLight;
using MediaBrowser.Windows8.Model;
using MetroLog;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SearchViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly ExtendedApiClient _apiClient;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        public SearchViewModel(NavigationService navigationService, ExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
            _logger = LogManagerFactory.DefaultLogManager.GetLogger<SearchViewModel>();

            if (IsInDesignMode)
            {
                
            }
        }
    }
}
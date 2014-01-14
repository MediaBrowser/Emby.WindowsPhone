using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GuideViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the GuideViewModel class.
        /// </summary>
        public GuideViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }
    }
}
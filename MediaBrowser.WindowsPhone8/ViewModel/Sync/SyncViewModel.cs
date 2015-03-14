using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model.Interfaces;

namespace MediaBrowser.WindowsPhone.ViewModel.Sync
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SyncViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SyncViewModel class.
        /// </summary>
        public SyncViewModel(INavigationService navigationService, IConnectionManager connectionManager) 
            : base(navigationService, connectionManager)
        {
        }
    }
}
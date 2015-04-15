using Emby.WindowsPhone.Model.Interfaces;
using MediaBrowser.Model.ApiClient;

namespace Emby.WindowsPhone.ViewModel.Offline
{
    public class OfflineUsersViewModel : ViewModelBase
    {
        public OfflineUsersViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }
    }
}

using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model.Interfaces;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    public class ViewModelBase : ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase
    {
        protected readonly INavigationService _navigationService;
        protected readonly IConnectionManager _connectionManager;

        protected IHasServerId ServerIdItem;

        public ViewModelBase(INavigationService navigationService, IConnectionManager connectionManager)
        {
            _navigationService = navigationService;
            _connectionManager = connectionManager;
        }

        protected IApiClient _apiClient
        {
            get { return _connectionManager.CurrentApiClient; }
        }
    }
}

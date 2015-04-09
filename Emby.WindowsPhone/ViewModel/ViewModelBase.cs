using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using Emby.WindowsPhone.Model.Interfaces;

namespace Emby.WindowsPhone.ViewModel
{
    public class ViewModelBase : ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase
    {
        protected readonly INavigationService NavigationService;
        protected readonly IConnectionManager ConnectionManager;

        protected IHasServerId ServerIdItem;

        public ViewModelBase(INavigationService navigationService, IConnectionManager connectionManager)
        {
            NavigationService = navigationService;
            ConnectionManager = connectionManager;
        }

        protected IApiClient ApiClient
        {
            get { return App.ServerInfo != null && !string.IsNullOrEmpty(App.ServerInfo.Id) ? ConnectionManager.GetApiClient(App.ServerInfo.Id) : ConnectionManager.CurrentApiClient; }
        }
    }
}

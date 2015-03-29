using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Model.Interfaces;

namespace MediaBrowser.WindowsPhone.ViewModel.Items
{
    public class SyncJobItemViewModel : ViewModelBase
    {
        public SyncJobItemViewModel(SyncJobItem syncJobItem, INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            SyncJobItem = syncJobItem;
        }

        public SyncJobItem SyncJobItem { get; set; }

        public RelayCommand DeleteSyncJobItemCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        await ApiClient.CancelSyncJobItem(SyncJobItem.Id);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("DeleteSyncJobItemCommand", ex, NavigationService, Log);
                    }
                });
            }
        }
    }
}

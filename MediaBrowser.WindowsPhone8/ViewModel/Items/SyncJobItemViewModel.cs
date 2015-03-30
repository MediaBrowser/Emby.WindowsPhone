using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;

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

        public string Name
        {
            get { return SyncJobItem != null && !string.IsNullOrEmpty(SyncJobItem.ItemName) ? SyncJobItem.ItemName : AppResources.LabelUntitled; }
        }

        public string Status
        {
            get
            {
                if (SyncJobItem == null)
                {
                    return string.Empty;
                }

                var id = string.Format("SyncJobStatus{0}", SyncJobItem.Status);

                return AppResources.ResourceManager.GetString(id);
            }
        }

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

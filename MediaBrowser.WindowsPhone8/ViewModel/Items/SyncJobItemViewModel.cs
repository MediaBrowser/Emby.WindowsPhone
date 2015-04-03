using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.WindowsPhone.Services;
using MediaBrowser.WindowsPhone.ViewModel.Sync;

namespace MediaBrowser.WindowsPhone.ViewModel.Items
{
    public class SyncJobItemViewModel : ViewModelBase
    {
        private readonly SyncJobDetailViewModel _syncJobDetailViewModel;

        public SyncJobItemViewModel(SyncJobItem syncJobItem, INavigationService navigationService, IConnectionManager connectionManager, SyncJobDetailViewModel syncJobDetailViewModel)
            : base(navigationService, connectionManager)
        {
            _syncJobDetailViewModel = syncJobDetailViewModel;
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

        public bool ActionIsVisible
        {
            get { return SyncJobItem != null && SyncJobItem.Status != SyncJobItemStatus.RemovedFromDevice; }
        }

        public string ActionText
        {
            get
            {
                var status = SyncJobItem.Status;
                switch (status)
                {
                    case SyncJobItemStatus.Cancelled:
                    case SyncJobItemStatus.Failed:
                        return AppResources.MenuRetry;
                    case SyncJobItemStatus.Queued:
                    case SyncJobItemStatus.Converting:
                    case SyncJobItemStatus.ReadyToTransfer:
                    case SyncJobItemStatus.Transferring:
                        return AppResources.MenuCancel;
                    case SyncJobItemStatus.Synced:
                        return SyncJobItem.IsMarkedForRemoval ? AppResources.MenuUnmarkForRemoval : AppResources.MenuMarkForRemoval;
                    default:
                        return string.Empty;
                }
            }
        }

        public RelayCommand ItemActionCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        var status = SyncJobItem.Status;
                        var id = SyncJobItem.Id;
                        switch (status)
                        {
                            case SyncJobItemStatus.Cancelled:
                                await ApiClient.EnableCancelledSyncJobItem(id);
                                break;
                            case SyncJobItemStatus.Failed:
                                await ApiClient.QueueFailedSyncJobItemForRetry(id);
                                break;
                            case SyncJobItemStatus.Queued:
                            case SyncJobItemStatus.Converting:
                            case SyncJobItemStatus.ReadyToTransfer:
                            case SyncJobItemStatus.Transferring:
                                await ApiClient.CancelSyncJobItem(id);
                                _syncJobDetailViewModel.SyncJobItems.Remove(this);
                                break;
                            case SyncJobItemStatus.Synced:
                                if (SyncJobItem.IsMarkedForRemoval)
                                {
                                    await ApiClient.UnmarkSyncJobItemForRemoval(id);
                                }
                                else
                                {
                                    await ApiClient.MarkSyncJobItemForRemoval(id);
                                }
                                break;
                        }

                        SyncService.Current.Sync().ConfigureAwait(false);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("ItemActionCommand(" + SyncJobItem.Status + ")", ex, NavigationService, Log);
                    }
                });
            }
        }
    }
}

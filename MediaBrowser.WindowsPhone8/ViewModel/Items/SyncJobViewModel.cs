using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.ViewModel.Sync;

namespace MediaBrowser.WindowsPhone.ViewModel.Items
{
    public class SyncJobViewModel : ViewModelBase
    {
        private readonly SyncViewModel _syncViewModel;

        public SyncJobViewModel(SyncJob syncJob, INavigationService navigationService, IConnectionManager connectionManager, SyncViewModel syncViewModel)
            : base(navigationService, connectionManager)
        {
            _syncViewModel = syncViewModel;
            SyncJob = syncJob;
        }

        public SyncJob SyncJob { get; set; }

        public string Name
        {
            get
            {
                return SyncJob != null && !string.IsNullOrEmpty(SyncJob.Name) ? SyncJob.Name : AppResources.LabelUntitled;
            }
        }

        public string ItemCount
        {
            get
            {
                if (SyncJob == null || SyncJob.ItemCount == 0)
                {
                    return string.Format(AppResources.LabelMultipleItems, 0);
                }

                return SyncJob.ItemCount == 1
                    ? AppResources.LabelOneItem
                    : string.Format(AppResources.LabelMultipleItems, SyncJob.ItemCount);
            }
        }

        public string Status
        {
            get
            {
                if (SyncJob == null)
                {
                    return string.Empty;
                }

                var id = string.Format("SyncJobStatus{0}", SyncJob.Status);

                return AppResources.ResourceManager.GetString(id);
            }
        }

        public string Id
        {
            get { return SyncJob != null ? SyncJob.Id : string.Empty; }
        }

        public RelayCommand NavigateToDetailsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SimpleIoc.Default.GetInstance<SyncJobDetailViewModel>() != null)
                    {
                        Messenger.Default.Send(new SyncJobMessage(this));
                    }

                    NavigationService.NavigateTo(Constants.Pages.Sync.SyncJobDetailView);
                });
            }
        }

        public RelayCommand DeleteSyncJobCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        await ApiClient.CancelSyncJob(SyncJob.Id);
                        _syncViewModel.SyncJobs.Remove(this);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("DeleteSyncJobCommand", ex, NavigationService, Log);
                    }
                });
            }
        }
    }
}

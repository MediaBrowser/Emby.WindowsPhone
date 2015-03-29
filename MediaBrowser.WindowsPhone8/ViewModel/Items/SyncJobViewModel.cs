using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;

namespace MediaBrowser.WindowsPhone.ViewModel.Items
{
    public class SyncJobViewModel : ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public SyncJobViewModel(SyncJob syncJob, INavigationService navigationService)
        {
            _navigationService = navigationService;
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
        
        public RelayCommand NavigateToDetailsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _navigationService.NavigateTo(Constants.Pages.Sync.SyncJobDetailView);
                });
            }
        }
    }
}

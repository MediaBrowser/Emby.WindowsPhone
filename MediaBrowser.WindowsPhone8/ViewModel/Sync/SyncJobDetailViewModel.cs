using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.ViewModel.Items;

namespace MediaBrowser.WindowsPhone.ViewModel.Sync
{
    public class SyncJobDetailViewModel : ViewModelBase
    {
        private SyncJob _syncJob;
        public SyncJobDetailViewModel(IConnectionManager connectionManager, INavigationService navigationService)
            : base(navigationService, connectionManager)
        {
            
        }

        public ObservableCollection<SyncJobItemViewModel> SyncJobItems { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingItems);

                var query = new SyncJobItemQuery
                {
                    JobId = _syncJob.Id,
                    TargetId = ConnectionManager.Device.DeviceId
                };
                var items = await ApiClient.GetSyncJobItems(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    var jobItems = items.Items.Select(x => new SyncJobItemViewModel(x, NavigationService, ConnectionManager));

                    SyncJobItems = new ObservableCollection<SyncJobItemViewModel>();
                    SyncJobItems.AddRange(jobItems);
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("LoadData()", ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<SyncJobMessage>(this, m =>
            {
                _syncJob = m.SyncJob;
                SyncJobItems = null;
            });
        }
    }
}

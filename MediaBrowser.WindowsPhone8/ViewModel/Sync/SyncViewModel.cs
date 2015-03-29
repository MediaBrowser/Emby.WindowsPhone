using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Services;
using MediaBrowser.WindowsPhone.ViewModel.Items;

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
        private bool _jobsLoaded;

        /// <summary>
        /// Initializes a new instance of the SyncViewModel class.
        /// </summary>
        public SyncViewModel(INavigationService navigationService, IConnectionManager connectionManager) 
            : base(navigationService, connectionManager)
        {
        }

        public ObservableCollection<SyncJobViewModel> SyncJobs { get; set; }

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

        public RelayCommand SyncJobsCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await SyncService.Current.Sync();
                });
            }
        }

        public RelayCommand NavigateToCurrentDownloadsCommand
        {
            get
            {
                return new RelayCommand(() => NavigationService.NavigateTo(Constants.Pages.Sync.CurrentDownloadsView));
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!isRefresh && _jobsLoaded)
            {
                return;
            }

            try
            {
                var jobs = await SyncService.Current.GetSyncJobs();

                SyncJobs = new ObservableCollection<SyncJobViewModel>();
                
                SyncJobs.AddRange(jobs.Select(x => new SyncJobViewModel(x, NavigationService, ConnectionManager)));

                _jobsLoaded = SyncJobs.Any();
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", NavigationService, Log);
            }
        }
    }
}
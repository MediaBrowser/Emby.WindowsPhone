using System.Collections.ObjectModel;
using System.Linq;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.ViewModel.Items;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.ViewModel.Sync
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CurrentDownloadsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CurrentDownloadsViewModel class.
        /// </summary>
        public CurrentDownloadsViewModel(INavigationService navigationService, IConnectionManager connectionManager) 
            : base(navigationService, connectionManager)
        {
        }

        public ObservableCollection<TransferMonitorViewModel> Transfers { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(LoadTransfers);
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadTransfers);
            }
        }

        public void LoadTransfers()
        {
            var requests = BackgroundTransferService.Requests.Select(TransferMonitorViewModel.Create).ToList();

            if (Transfers == null)
            {
                Transfers = new ObservableCollection<TransferMonitorViewModel>(requests);
            }
            else
            {
                var newRequests = requests.Except(Transfers).ToList();
                Transfers.AddRange(newRequests);
            }
        }
    }
}
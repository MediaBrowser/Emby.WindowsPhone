using System.Collections.ObjectModel;
using System.Linq;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model.Interfaces;
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

        public ObservableCollection<TransferMonitor> Transfers { get; set; }

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
            var requests = BackgroundTransferService.Requests.Select(x => new TransferMonitor(x)).ToList();

            if (Transfers == null)
            {
                Transfers = new ObservableCollection<TransferMonitor>(requests);
            }
            else
            {
                var newRequests = requests.Except(Transfers).ToList();
                Transfers.AddRange(newRequests);
            }
        }
    }
}
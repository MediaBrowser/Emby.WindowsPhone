using MediaBrowser.WindowsPhone.Model.Sync;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace MediaBrowser.WindowsPhone.ViewModel.Items
{
    public class TransferMonitorViewModel : ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase
    {
        private TransferMonitorViewModel(TransferMonitor monitor)
        {
            Monitor = monitor;
        }

        public static TransferMonitorViewModel Create(BackgroundTransferRequest request)
        {
            var vm = new TransferMonitorViewModel(new TransferMonitor(request));
            var json = request.Tag;
            var jobData = JsonConvert.DeserializeObject<JobData>(json);
            vm.JobData = jobData;

            vm.Monitor.Name = jobData.Name;

            return vm;
        }

        public TransferMonitor Monitor { get; set; }
        public JobData JobData { get; set; }

        public string DisplayName
        {
            get { return JobData != null ? JobData.Name : "Download Item"; }
        }
    }
}

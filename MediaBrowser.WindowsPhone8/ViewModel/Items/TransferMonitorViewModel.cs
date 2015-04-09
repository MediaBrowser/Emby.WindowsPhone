using Emby.WindowsPhone.Model.Sync;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace Emby.WindowsPhone.ViewModel.Items
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
            vm.CreateJobData(request);

            return vm;
        }

        private void CreateJobData(BackgroundTransferRequest request)
        {
            var json = request.Tag;
            var jobData = JsonConvert.DeserializeObject<JobData>(json);
            JobData = jobData;

            Monitor.Name = jobData.Name;
        }

        public TransferMonitor Monitor { get; set; }
        public JobData JobData { get; set; }

        public string DisplayName
        {
            get { return JobData != null ? JobData.Name : "Download Item"; }
        }
    }
}

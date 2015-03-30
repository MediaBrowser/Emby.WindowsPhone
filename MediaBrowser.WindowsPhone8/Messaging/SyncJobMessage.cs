using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.ViewModel.Items;

namespace MediaBrowser.WindowsPhone.Messaging
{
    public class SyncJobMessage : MessageBase
    {
        public SyncJobViewModel SyncJob { get; set; }
        public SyncJobMessage(SyncJobViewModel syncJob)
        {
            SyncJob = syncJob;
        }
    }
}
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Messaging
{
    public class SyncJobMessage : MessageBase
    {
        public SyncJob SyncJob { get; set; }
        public SyncJobMessage(SyncJob syncJob)
        {
            SyncJob = syncJob;
        }
    }
}
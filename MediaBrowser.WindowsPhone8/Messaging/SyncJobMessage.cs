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

    public class SyncNotificationMessage : NotificationMessage
    {
        public string ItemId { get; private set; }

        public SyncNotificationMessage(string notification, string itemId) : base(notification)
        {
            ItemId = itemId;
        }

        public SyncNotificationMessage(object sender, string notification, string itemId)
            : base(sender, notification)
        {
            ItemId = itemId;
        }

        public SyncNotificationMessage(object sender, object target, string notification, string itemId)
            : base(sender, target, notification)
        {
            ItemId = itemId;
        }
    }
}
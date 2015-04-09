using GalaSoft.MvvmLight.Messaging;
using Emby.WindowsPhone.ViewModel.Items;

namespace Emby.WindowsPhone.Messaging
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
        public string ItemType { get; private set; }

        public SyncNotificationMessage(string notification, string itemId, string itemType) : base(notification)
        {
            ItemId = itemId;
            ItemType = itemType;
        }

        public SyncNotificationMessage(object sender, string notification, string itemId, string itemType)
            : base(sender, notification)
        {
            ItemId = itemId;
            ItemType = itemType;
        }

        public SyncNotificationMessage(object sender, object target, string notification, string itemId, string itemType)
            : base(sender, target, notification)
        {
            ItemId = itemId;
            ItemType = itemType;
        }
    }
}
using GalaSoft.MvvmLight.Messaging;
using Emby.WindowsPhone.Interfaces;
using Emby.WindowsPhone.Messaging;

namespace Emby.WindowsPhone.Services
{
    public class MessengerService : IMessengerService
    {
        public void SendNotification(string notification, object sender = null, object target = null)
        {
            Messenger.Default.Send(new NotificationMessage(sender, target, notification));
        }

        public void SendSyncNotification(string notification, string itemId, string itemType, object sender = null, object target = null)
        {
            Messenger.Default.Send(new SyncNotificationMessage(sender, target, notification, itemId, itemType));
        }

        public void SendAppResetNotification()
        {
            Messenger.Default.Send(new ResetAppMessage());
        }
    }
}
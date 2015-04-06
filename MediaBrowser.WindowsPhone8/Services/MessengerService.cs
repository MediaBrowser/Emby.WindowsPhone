using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.Interfaces;
using MediaBrowser.WindowsPhone.Messaging;

namespace MediaBrowser.WindowsPhone.Services
{
    public class MessengerService : IMessengerService
    {
        public void SendNotification(string notification, object sender = null, object target = null)
        {
            Messenger.Default.Send(new NotificationMessage(sender, target, notification));
        }

        public void SendSyncNotification(string notification, string itemId, object sender = null, object target = null)
        {
            Messenger.Default.Send(new SyncNotificationMessage(sender, target, notification, itemId));
        }
    }
}
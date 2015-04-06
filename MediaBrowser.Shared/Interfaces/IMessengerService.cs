namespace MediaBrowser.WindowsPhone.Interfaces
{
    public interface IMessengerService
    {
        void SendNotification(string notification, object sender = null, object target = null);
        void SendSyncNotification(string notification, string itemId, object sender = null, object target = null);
    }
}
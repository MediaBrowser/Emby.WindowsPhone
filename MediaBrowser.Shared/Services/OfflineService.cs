namespace MediaBrowser.WindowsPhone.Services
{
    public class OfflineService
    {
        public static OfflineService Current { get; private set; }

        static OfflineService()
        {
            Current = new OfflineService();
        }

        public OfflineService()
        {
            Current = this;
        }

        private bool RequiresMoreSpace(float requestedSpace)
        {

            return false;
        }
    }
}

using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class SyncRequest : SyncJobRequest
    {
        public bool DisplayNewerItemsSync { get; set; }
        public bool DisplayLimit { get; set; }
        public bool DisplayUnwatchedItemsSync { get; set; }
    }
}

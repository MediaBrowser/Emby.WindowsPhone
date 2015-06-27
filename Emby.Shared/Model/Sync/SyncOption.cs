using MediaBrowser.Model.Sync;

namespace Emby.WindowsPhone.Model.Sync
{
    public class SyncOption
    {
        public bool AutoSyncNewItems { get; set; }
        public bool UnwatchedItems { get; set; }
        public int? ItemLimit { get; set; }
        public SyncQualityOption Quality { get; set; }
        public SyncProfileOption Profile { get; set; }
    }
}

using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class SyncOption
    {
        public bool AutoSyncNewItems { get; set; }
        public bool UnwatchedItems { get; set; }
        public int? ItemLimit { get; set; }
        public SyncQualityOption Quality { get; set; }
    }
}

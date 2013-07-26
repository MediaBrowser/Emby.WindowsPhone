using System.Collections.Generic;
using PropertyChanged;

namespace MediaBrowser.Model
{
    [ImplementPropertyChanged]
    public class DeviceSettings
    {
        public string DeviceId { get; set; }
        public bool SendToasts { get; set; }
        public bool SendLiveTiles { get; set; }
        public List<LiveTile> LiveTiles { get; set; }
    }
}

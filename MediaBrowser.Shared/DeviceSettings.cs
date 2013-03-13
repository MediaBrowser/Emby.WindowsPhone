using System.Collections.Generic;
using System.ComponentModel;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.Shared
{
    public class DeviceSettings : INotifyPropertyChanged
    {
        public string DeviceId { get; set; }
        public bool SendToasts { get; set; }
        public bool SendLiveTiles { get; set; }
        public List<LiveTile> LiveTiles { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

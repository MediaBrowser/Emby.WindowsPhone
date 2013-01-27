using System.ComponentModel;

namespace MediaBrowser.WindowsPhone.Model
{
    public class LiveTile : INotifyPropertyChanged
    {
        public string TileName { get; set; }
        public string LiveTileId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
using PropertyChanged;

namespace MediaBrowser.Model
{
    [ImplementPropertyChanged]
    public class LiveTile
    {
        public string TileName { get; set; }
        public string LiveTileId { get; set; }
    }
}
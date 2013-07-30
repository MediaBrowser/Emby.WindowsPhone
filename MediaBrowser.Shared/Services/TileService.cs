using System.Linq;
using Cimbalino.Phone.Toolkit.Services;

namespace MediaBrowser.Services
{
    public class TileService : ShellTileService
    {
        private static TileService _current;

        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public bool TileExists(string uri)
        {
            var tile = ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Equals(uri));
            return tile != default(ShellTileServiceTile);
        }

        public IShellTileServiceTile GetTile(string uri)
        {
            var tile = ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Equals(uri));
            return tile;
        }
    }
}

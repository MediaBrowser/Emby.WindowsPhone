using System.Linq;
using Cimbalino.Phone.Toolkit.Services;

namespace MediaBrowser.WindowsPhone.Services
{
    public class TileService : ShellTileWithCreateService 
    {
        private static TileService _current;

        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public bool TileExists(string uri)
        {
            var tile = ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(uri));
            return tile != default(ShellTileServiceTile);
        }

        public IShellTileServiceTile GetTile(string uri)
        {
            var tile = ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(uri));
            return tile;
        }
    }
}

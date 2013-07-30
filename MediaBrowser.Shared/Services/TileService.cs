using System.Linq;
using Cimbalino.Phone.Toolkit.Services;

namespace MediaBrowser.Services
{
    public class TileService
    {
        private static TileService _current;
        private static readonly ShellTileService ShellTileService = new ShellTileService();

        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public bool TileExists(string uri)
        {
            var tile = ShellTileService.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(uri));
            return tile != default(ShellTileServiceTile);
        }
    }
}

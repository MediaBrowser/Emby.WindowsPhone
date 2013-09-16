using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Services
{
    public class TileService : ShellTileWithCreateService
    {
        private readonly ILog _logger; 
        private static TileService _current;

        public TileService()
        {
            _logger = new WPLogger(typeof(TileService));
        }
        
        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public IDictionary<string, string> PinnedUrlQuery { get; set; }

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

        public string PinnedPage()
        {
            if (PinnedUrlQuery.IsNullOrEmpty() || !PinnedUrlQuery.ContainsKey("action"))
            {
                _logger.Debug("No pinned secondary tile");
                return Constants.Pages.MainPage;
            }

            var action = PinnedUrlQuery.GetValue("action");

            switch (action.ToLower(CultureInfo.InvariantCulture))
            {
                case "collection":
                    string name, id;
                    if (PinnedUrlQuery.TryGetValue("name", out name) &&
                        PinnedUrlQuery.TryGetValue("id", out id))
                    {
                        _logger.Info("Collection pinned tile");
                        var navigationUrl = string.Format("/Views/CollectionView.xaml?id={1}&name={2}", action, id, name);
                        return navigationUrl;
                    }
                    break;
                case "remote":
                    _logger.Info("Remote pinned tile");
                    return Constants.Pages.Remote.RemoteView;
                default:
                    return Constants.Pages.MainPage;
            }

            return Constants.Pages.MainPage;
        }
    }
}

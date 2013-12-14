using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Controls;
using ScottIsAFool.WindowsPhone.Logging;
using MediaBrowser.Model.ApiClient;

namespace MediaBrowser.WindowsPhone.Services
{
    public class TileService : ShellTileWithCreateService
    {
        private readonly ILog _logger; 
        private static TileService _current;

        private IExtendedApiClient _apiClient;
        private IAsyncStorageService _storageService;

        private const string WideTileUrl = "shared\\shellcontent\\WideTileImage.png";

        public TileService()
        {
            _logger = new WPLogger(typeof(TileService));
        }
        
        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public void StartService(IExtendedApiClient apiClient, IAsyncStorageService storageService)
        {
            _logger.Info("Starting TileService");
            _apiClient = apiClient;
            _storageService = storageService;
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

        public async Task CreateNewWideTileAsync()
        {
            var items = await GetCollectionItems(9);
            if (items == null || items.Items.IsNullOrEmpty())
            {
                return;
            }

            var list = new List<Stream>();
            foreach (var item in items.Items)
            {
                var url = _apiClient.GetImageUrl(item, new ImageOptions
                {
                    ImageType = ImageType.Primary,
                    MaxWidth = 112,
                    Quality = 90,
                    EnableImageEnhancers = false
                });

                try
                {
                    var stream = await _apiClient.GetImageStreamAsync(url);
                    list.Add(stream);
                }
                catch (HttpException ex)
                {
                    _logger.ErrorException("CreateNewWideTileAsync()", ex);
                }

                if (list.IsNullOrEmpty())
                {
                    _logger.Debug("No images found, wide tile not changed");
                    return;
                }
            }

            var wideTile = new WideTileControl
            {
                ItemsSource = list,
                Height = 336,
                Width = 691
            };
            wideTile.UpdateBackground();
            await ToImage(wideTile);
        }

        private async Task ToImage(UIElement element)
        {
            element.Measure(new Size(691, 336));
            element.Arrange(new Rect { Height = 336, Width = 691 });
            element.UpdateLayout();

            var bitmap = new WriteableBitmap(691, 336);
            bitmap.Render(element, null);
            bitmap.Invalidate();

            await SaveTheImage(bitmap, WideTileUrl);
        }

        private async Task SaveTheImage(WriteableBitmap bitmap, string filename)
        {
            if (await _storageService.FileExistsAsync(filename))
            {
                await _storageService.DeleteFileAsync(filename);
            }

            using (var fileStream = await _storageService.CreateFileAsync(filename))
            {
                bitmap.SavePng(fileStream);
            }
        }

        private async Task<ItemsResult> GetCollectionItems(int limit)
        {
            var query = new ItemQuery
            {
                IncludeItemTypes = new[] { "Season", "Series", "Movie", "Album" },
                Limit = limit,
                SortBy = new[] { ItemSortBy.Random },
                UserId = AuthenticationService.Current.LoggedInUser.Id,
                ImageTypes = new[] { ImageType.Primary },
                Recursive = true
            };

            var itemResponse = await _apiClient.GetItemsAsync(query);
            return itemResponse;
        }
    }
}

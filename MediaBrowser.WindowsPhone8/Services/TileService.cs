using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Helpers;
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

#if WP8
using MediaBrowser.WindowsPhone.Extensions;
#endif

namespace MediaBrowser.WindowsPhone.Services
{
    public class TileService : ShellTileWithCreateService
    {
        private readonly ILog _logger;
        private static TileService _current;

        private IExtendedApiClient _apiClient;
        private IAsyncStorageService _storageService;

        private const string WideTileUrl = "shared\\shellcontent\\WideTileImage.png";
        private const string WideTileBackUrl = "shared\\shellcontent\\WideTileBackImage.png";
        private const string MediumTileBackUrl = "shared\\shellcontent\\MediumTileBackImage.png";

        private const string DefaultWideTileUrl = "/Assets/Tiles/FlipCycleTileLarge.png";
        private const string WideTileActualUrl = "isostore:/" + WideTileUrl;
        private const string WideTileBackActualUrl = "isostore:/" + WideTileBackUrl;
        private const string MediumTileActualUrl = "isostore:/" + MediumTileBackUrl;

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
                case "livetv":
                    _logger.Info("Live TV pinned tile");
                    return Constants.Pages.LiveTv.LiveTvView;
                default:
                    return Constants.Pages.MainPage;
            }

            return Constants.Pages.MainPage;
        }

#if WP8
        public async Task UpdatePrimaryTile(bool displayBackdropOnTile, bool useRichWideTile)
        {
            Uri wideBackUri, wideUri, mediumBackUri;
            if (displayBackdropOnTile)
            {
                await CreateNewWideTileAsync();
            }

            if (useRichWideTile)
            {
                await UpdateBackContentImages();
            }

            wideUri = useRichWideTile
                ? new Uri(WideTileActualUrl, UriKind.Absolute)
                : new Uri(DefaultWideTileUrl, UriKind.Relative);

            wideBackUri = displayBackdropOnTile
                ? new Uri(WideTileBackActualUrl, UriKind.Absolute)
                : new Uri("/", UriKind.Relative);

            mediumBackUri = displayBackdropOnTile
                ? new Uri(MediumTileActualUrl, UriKind.Absolute)
                : new Uri("/", UriKind.Relative);

            UpdateTileData(wideUri, mediumBackUri, wideBackUri);
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
            await wideTile.SetImages();
            await ToImage(wideTile, 691, 336);
        }

        public void ResetWideTile()
        {
            UpdateTileData(new Uri(DefaultWideTileUrl, UriKind.Relative));
        }

        public async Task UpdateBackContentImages()
        {
            var items = await GetCollectionItems(1, ImageType.Backdrop);
            if (items == null || items.Items.IsNullOrEmpty())
            {
                return;
            }

            var item = items.Items.FirstOrDefault();
            var wideUrl = _apiClient.GetImageUrl(item, new ImageOptions
            {
                ImageType = ImageType.Backdrop,
                MaxWidth = 691,
                Quality = 95,
                EnableImageEnhancers = false
            });

            try
            {
                var stream = await _apiClient.GetImageStreamAsync(wideUrl);
                var imageSource = new BitmapImage();
                imageSource.SetSource(stream);

                var writeableImage = new WriteableBitmap(imageSource);
                await SaveTheImage(writeableImage, WideTileBackUrl);

                var mediumWriteableImage = writeableImage.CentreCrop(336, 336);
                await SaveTheImage(mediumWriteableImage, MediumTileBackUrl);
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("UpdateBackContentImages()", ex);
            }
        }

        private void UpdateTileData(Uri wideTileUri, Uri backContentUri = null, Uri backContentWideuri = null)
        {
            var primaryTile = ActiveTiles.First();

            var tileData = new ShellTileServiceFlipTileData
            {
                Title = ApplicationManifest.Current.App.Title,
                BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative),
                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative),
                WideBackgroundImage = wideTileUri,
                BackBackgroundImage = backContentUri ?? new Uri("/", UriKind.Relative),
                WideBackBackgroundImage = backContentWideuri ?? new Uri("/", UriKind.Relative)
            };

            primaryTile.Update(tileData);
        }

        private async Task ToImage(UIElement element, double width, double height)
        {
            element.Measure(new Size(width, height));
            element.Arrange(new Rect { Height = height, Width = width });
            element.UpdateLayout();

            var bitmap = new WriteableBitmap((int)width, (int)height);
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

        private async Task<ItemsResult> GetCollectionItems(int limit, ImageType imageType = ImageType.Primary)
        {
            var query = new ItemQuery
            {
                IncludeItemTypes = new[] { "Season", "Series", "Movie", "Album" },
                Limit = limit,
                SortBy = new[] { ItemSortBy.Random },
                UserId = AuthenticationService.Current.LoggedInUser.Id,
                ImageTypes = new[] { imageType },
                Recursive = true
            };

            try
            {
                var itemResponse = await _apiClient.GetItemsAsync(query);
                return itemResponse;
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("GetCollectionItems(" + imageType + ")", ex);
            }

            return null;
        }
#endif
    }
}

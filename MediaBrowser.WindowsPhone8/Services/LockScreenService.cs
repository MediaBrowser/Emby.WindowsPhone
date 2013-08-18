using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using ImageTools;
using ImageTools.IO.Png;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Controls;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.Logging;
using MediaBrowser.Model.ApiClient;

namespace MediaBrowser.WindowsPhone.Services
{
    public class LockScreenService : Cimbalino.Phone.Toolkit.Services.LockScreenService
    {
        private const string LockScreenImageUrlNormal = "ms-appdata:///Local/shared/shellcontent/MBWallpaper.png";
        private const string LockScreenImageUrlAlternative = "ms-appdata:///Local/shared/shellcontent/MBWallpaper2.png";
        private const string LockScreenFileNormal = "shared\\shellcontent\\MBWallpaper.png";
        private const string LockScreenFileAlternative = "shared\\shellcontent\\MBWallpaper2.png";
        private const string DefaultLockScreenImageFormat = "ms-appx:///DefaultLockScreen.jpg";
        private static LockScreenService _current;
        private readonly IAsyncStorageService _storageService = new AsyncStorageService();
        private readonly ILog _logger = new WPLogger(typeof(LockScreenService));
        private IExtendedApiClient _apiClient;

        private bool _serviceStarted;
        
        public static LockScreenService Current { get { return _current ?? (_current = new LockScreenService()); }}

        public string LockScreenImageUrl
        {
            get
            {
                return ImageUri.ToString().EndsWith("2.png") ? LockScreenImageUrlNormal : LockScreenImageUrlAlternative;
            }
        }

        public string LockScreenFile
        {
            get
            {
                return ImageUri.ToString().EndsWith("2.png") ? LockScreenFileNormal : LockScreenFileAlternative;
            }
        }

        public ImageOptions SinglePosterOptions
        {
            get
            {
                return new ImageOptions
                {
                    ImageType = ImageType.Primary,
                    MaxWidth = 480,
                    Quality = 90,
                    EnableImageEnhancers = false
                };
            }
        }

        public ImageOptions MultiplePostersOptions
        {
            get
            {
                return new ImageOptions
                {
                    ImageType = ImageType.Primary,
                    MaxWidth = 167,
                    Quality = 90,
                    EnableImageEnhancers = false
                };
            }
        }

        public ImageOptions CollageOptions
        {
            get
            {
                return new ImageOptions
                {
                    ImageType = ImageType.Primary,
                    MaxWidth = 160,
                    Quality = 90,
                    EnableImageEnhancers = false
                };
            }
        }

        public bool ManuallySet { get; set; }
        public string CollectionId { get; set; }

        public void Start()
        {
            _apiClient = SimpleIoc.Default.GetInstance<IExtendedApiClient>(); 
            _serviceStarted = true;
        }

        public async Task SetLockScreen(LockScreenType lockScreenType)
        {
            if (!_serviceStarted)
            {
                return;
            }

            switch (lockScreenType)
            {
                case LockScreenType.Default:
                    await SetDefaultLockscreenImage();
                    break;
                case LockScreenType.SinglePoster:
                    if (ManuallySet)
                    {
                        ManuallySet = false;
                        return;
                    }
                    break;
                case LockScreenType.MultiplePosters:
                    await SetMultipleImagesLockScreen();
                    break;
                case LockScreenType.FullScreenCollage:
                    await SetCollageLockScreen();
                    break;
            }
        }

        private async Task SetCollageLockScreen()
        {
            try
            {
                var itemResponse = await GetCollectionItems(12);

                if (itemResponse != null && !itemResponse.Items.IsNullOrEmpty())
                {
                    await ProcessCollageImages(itemResponse.Items);
                }
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("SetCollageLockScreen()", ex);
            }
        }

        private async Task SetMultipleImagesLockScreen()
        {
            try
            {
                var itemResponse = await GetCollectionItems(5);

                if (itemResponse != null && !itemResponse.Items.IsNullOrEmpty())
                {
                    await ProcessMultipleImages(itemResponse.Items);
                }
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("SetMultipleImagesLockScreen()", ex);
            }
        }

        private async Task<ItemsResult> GetCollectionItems(int limit)
        {
            var query = new ItemQuery
            {
                IncludeItemTypes = new[] {"Season", "Series", "Movie"},
                Limit = limit,
                SortBy = new[] {ItemSortBy.Random},
                UserId = AuthenticationService.Current.LoggedInUser.Id,
                ImageTypes = new []{ImageType.Primary},
                Recursive = true
            };

            if (!string.IsNullOrEmpty(CollectionId))
            {
                query.ParentId = CollectionId;
            }

            var itemResponse = await _apiClient.GetItemsAsync(query);
            return itemResponse;
        }

        private async Task ProcessMultipleImages(IEnumerable<BaseItemDto> items)
        {
            var list = new List<Stream>();

            foreach (var item in items)
            {
                var url = _apiClient.GetImageUrl(item, MultiplePostersOptions);
                try
                {
                    var stream = await _apiClient.GetImageStreamAsync(url);
                    list.Add(stream);
                }
                catch (HttpException ex)
                {
                    _logger.ErrorException("ProcessMultipleImages()", ex);
                }
            }

            if (list.IsNullOrEmpty())
            {
                _logger.Debug("No images found, lockscreen image not changed");
                return;
            }

            var lockscreen = new LockScreenMultiImage {ItemsSource = list, Height = 800, Width = 480};
            
            await ToImage(lockscreen);

            await SetLockScreenImage(LockScreenImageUrl);
        }

        private async Task ProcessCollageImages(IEnumerable<BaseItemDto> items)
        {
            var list = new List<Stream>();

            foreach (var item in items)
            {
                var url = _apiClient.GetImageUrl(item, CollageOptions);
                try
                {
                    var stream = await _apiClient.GetImageStreamAsync(url);
                    list.Add(stream);
                }
                catch (HttpException ex)
                {
                    _logger.ErrorException("ProcessCollageImages()", ex);
                }
            }

            if (list.IsNullOrEmpty())
            {
                _logger.Debug("No images found, lockscreen image not changed");
                return;
            }

            var lockscreen = new LockScreenCollage { ItemsSource = list, Height = 800, Width = 480 };

            await ToImage(lockscreen);

            await SetLockScreenImage(LockScreenImageUrl);
        }

        private async Task ToImage(UIElement element)
        {
            element.Measure(new Size(480, 800));
            element.Arrange(new Rect{ Height = 800, Width = 480});
            //element.InvalidateMeasure();
            //element.InvalidateArrange();
            element.UpdateLayout();

            var bitmap = new WriteableBitmap(480, 800);
            bitmap.Render(element, null);
            bitmap.Invalidate();

            await SaveTheImage(bitmap);
        }

        private async Task SaveTheImage(WriteableBitmap bitmap)
        {
            if (await _storageService.FileExistsAsync(LockScreenFile))
            {
                await _storageService.DeleteFileAsync(LockScreenFile);
            }

            using (var fileStream = await _storageService.CreateFileAsync(LockScreenFile))
            {
                var encoder = new PngEncoder();
                var image = bitmap.ToImage();
                encoder.Encode(image, fileStream);
            }
        }
        
        public async Task SetLockScreenImage(string uri)
        {
            if (uri.StartsWith("http"))
            {
                await DownloadImage(uri);
                ImageUri = new Uri(LockScreenImageUrl, UriKind.RelativeOrAbsolute);
            }
            else
            {
                ImageUri = new Uri(uri, UriKind.RelativeOrAbsolute);
            }
        }

        public async Task SetDefaultLockscreenImage()
        {
            await SetLockScreenImage(DefaultLockScreenImageFormat);
        }

        private async Task DownloadImage(string uri)
        {
            try
            {
                var bitmap = new BitmapImage();
                var client = new HttpClient();
                var stream = await client.GetAsync(uri);
                bitmap.SetSource(await stream.Content.ReadAsStreamAsync());
                var writeableBitmap = new WriteableBitmap(bitmap);

                await SaveTheImage(writeableBitmap);

                await SetLockScreenImage(LockScreenImageUrl);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("DownloadImage()", ex);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Services;
using ImageTools;
using ImageTools.IO.Png;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Services
{
    public class LockScreenService : Cimbalino.Phone.Toolkit.Services.LockScreenService
    {
        private const string LockScreenImageUrl = "ms-appdata:///Local/shared/shellcontent/MBWallpaper.png";
        private const string LockScreenImageUrl2 = "ms-appdata:///Local/shared/shellcontent/MBWallpaper2.png";
        private const string LockScreenFile = "shared\\shellcontent\\MBWallpaper.png";
        private const string LockScreenFile2 = "shared\\shellcontent\\MBWallpaper2.png";
        private const string DefaultLockScreenImageFormat = "ms-appx:///DefaultLockScreen.jpg";
        private static LockScreenService _current;
        private readonly IAsyncStorageService _storageService = new AsyncStorageService();
        private readonly ILog _logger = new WPLogger(typeof(LockScreenService));

        public static LockScreenService Current { get { return _current ?? (_current = new LockScreenService()); }}

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
                    MaxWidth = 120,
                    Quality = 90,
                    EnableImageEnhancers = false
                };
            }
        }

        public bool ManuallySet { get; set; }

        public async Task SetLockScreen(LockScreenType lockScreenType)
        {
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

                    break;
            }
        }

        public async Task SetLockScreenImage(string uri)
        {
            if (uri.StartsWith("http"))
            {
                await DownloadImage(uri);
                var fileName = ImageUri.ToString().EndsWith("2.png") ? LockScreenImageUrl : LockScreenImageUrl2;
                ImageUri = new Uri(fileName, UriKind.RelativeOrAbsolute);
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

                var fileName = ImageUri.ToString().EndsWith("2.png") ? LockScreenFile : LockScreenFile2;

                using (var fileStream = await _storageService.CreateFileAsync(fileName))
                {
                    var encoder = new PngEncoder();
                    var image = writeableBitmap.ToImage();
                    encoder.Encode(image, fileStream);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("DownloadImage()", ex);
            }
        }
    }
}

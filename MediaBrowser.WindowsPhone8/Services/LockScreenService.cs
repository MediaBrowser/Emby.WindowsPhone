using System;
using System.Net.Http;
using System.Threading;
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

namespace MediaBrowser.WindowsPhone.Services
{
    public class LockScreenService : Cimbalino.Phone.Toolkit.Services.LockScreenService
    {
        private const string LockScreenImageUrl = "isostore:/shared/shellcontent/MBWallpaper.png";
        private const string LockScreenFile = "shared\\shellcontent\\MBWallpaper.png";
        private const string DefaultLockScreenImageFormat = "ms-appx:///DefaultLockScreen.jpg";
        private static LockScreenService _current;
        private readonly IAsyncStorageService _storageService = new AsyncStorageService();

        public static LockScreenService Current { get { return _current ?? (_current = new LockScreenService()); }}

        public ImageOptions LockScreenImageOptions
        {
            get
            {
                return new ImageOptions
                {
                    ImageType = ImageType.Primary,
                    MaxWidth = 480,
                    Quality = 90
                };
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
            var bitmap = new BitmapImage();
            var client = new HttpClient();
            var stream = await client.GetAsync(uri);
            bitmap.SetSource(await stream.Content.ReadAsStreamAsync());
            var writeableBitmap = new WriteableBitmap(bitmap);

            using (var fileStream = await _storageService.CreateFileAsync(LockScreenFile))
            {
                var encoder = new PngEncoder();
                var image = writeableBitmap.ToImage();
                encoder.Encode(image, fileStream);
            }
        }
    }
}

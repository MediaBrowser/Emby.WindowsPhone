using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Phone.System.UserProfile;
using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls.Common;
using ImageTools;
using ImageTools.IO.Png;

namespace MediaBrowser.WindowsPhone.Services
{
    public class LockScreenService
    {
        private const string LockScreenImageUrl = "ms-appx:///shared/shellcontent/MBWallpaper.png";
        private const string LockScreenFile = "shared/shellcontent/MBWallpaper.png";
        private const string DefaultLockScreenImageFormat = "ms-appx:///Images/LockScreenDefault.{0}.png";
        private static LockScreenService _current;
        private readonly IAsyncStorageService _storageService = new AsyncStorageService();

        public static LockScreenService Current { get { return _current ?? (_current = new LockScreenService()); }}

        public bool IsProvider { get { return LockScreenManager.IsProvidedByCurrentApplication; }}

        public async Task RequestToBeProvider()
        {
            await LockScreenManager.RequestAccessAsync();
        }

        public async Task SetLockScreenImage(string uri)
        {
            if (uri.StartsWith("http"))
            {
                await DownloadImage(uri);
                LockScreen.SetImageUri(new Uri(LockScreenImageUrl, UriKind.RelativeOrAbsolute));
            }
            else
            {
                LockScreen.SetImageUri(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
        }

        public Uri GetLockScreenUri()
        {
            return LockScreen.GetImageUri();
        }

        public async Task SetDefaultLockscreenImage()
        {
            var scale = ApplicationSpace.ScaleFactor();
            var imageSize = scale == 150 ? "720p" : "WXGA";

            var imageUrl = string.Format(DefaultLockScreenImageFormat, imageSize);

            await SetLockScreenImage(imageUrl);
        }

        private async Task DownloadImage(string uri)
        {
            var bitmap = new BitmapImage();
            await Task.Run(() =>
            {
                bitmap = new BitmapImage(new Uri(uri));
            });
            bitmap.CreateOptions = BitmapCreateOptions.None;

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

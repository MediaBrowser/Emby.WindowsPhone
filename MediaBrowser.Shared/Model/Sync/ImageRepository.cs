using System.IO;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class ImageRepository : IImageRepository
    {
        private readonly IAsyncStorageService _storageService;
        private const string ImageCachePath = "Cache\\Images";
        private const string ImageCacheItemPath = ImageCachePath + "\\{0}\\"; // {0} = itemId
        private const string ImageCacheItemImage = "{1}{0}.jpg";

        public ImageRepository(IAsyncStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task SaveImage(string itemId, string imageId, Stream stream)
        {
            var itemCacheFolder = GetItemCachePath(itemId);
            await CheckAndCreateCacheFolder(itemCacheFolder);

            var imageFile = GetItemImagePath(itemCacheFolder, imageId);
            await _storageService.WriteAllBytesAsync(imageFile, await stream.ToArrayAsync());
        }

        public async Task<Stream> GetImage(string itemId, string imageId)
        {
            var itemCacheFolder = GetItemCachePath(itemId);

            var imageFile = GetItemImagePath(itemCacheFolder, imageId);
            if (!await _storageService.FileExistsAsync(imageFile))
            {
                return await _storageService.OpenFileForReadAsync(imageFile);
            }

            return null;
        }

        public async Task DeleteImage(string itemId, string imageId)
        {
            var itemCacheFolder = GetItemCachePath(itemId);

            var imageFile = GetItemImagePath(itemCacheFolder, imageId);
            if (!await _storageService.FileExistsAsync(imageFile))
            {
                await _storageService.DeleteFileAsync(imageFile);
            }
        }

        public async Task<bool> HasImage(string itemId, string imageId)
        {
            var itemCacheFolder = GetItemCachePath(itemId);

            var imageFile = GetItemImagePath(itemCacheFolder, imageId);
            return await _storageService.FileExistsAsync(imageFile);
        }

        public async Task DeleteImages(string itemId)
        {
            var itemCacheFolder = GetItemCachePath(itemId);

            if (!await _storageService.DirectoryExistsAsync(itemCacheFolder))
            {
                await _storageService.DeleteDirectoryAsync(itemCacheFolder);
            }
        }

        private static string GetItemCachePath(string itemId)
        {
            return string.Format(ImageCacheItemPath, itemId);
        }

        private static string GetItemImagePath(string cachePath, string imageId)
        {
            return string.Format(ImageCacheItemImage, cachePath, imageId);
        }

        private async Task CheckAndCreateCacheFolder(string path = null)
        {
            await CheckAndCreateDirectory("Cache");
            await CheckAndCreateDirectory(ImageCachePath);

            if (!string.IsNullOrEmpty(path))
            {
                await CheckAndCreateDirectory(path);
            }
        }

        private async Task CheckAndCreateDirectory(string path)
        {
            if (!await _storageService.DirectoryExistsAsync(path))
            {
                await _storageService.CreateDirectoryAsync(path);
            }
        }
    }
}
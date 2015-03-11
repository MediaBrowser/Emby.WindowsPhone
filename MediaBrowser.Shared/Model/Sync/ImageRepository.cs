using System.IO;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.WindowsPhone.Extensions;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class ImageRepository : IImageRepository
    {
        private readonly IStorageServiceHandler _storageService;
        private const string ImageCachePath = "Cache\\Images";
        private const string ImageCacheItemPath = ImageCachePath + "\\{0}\\"; // {0} = itemId
        private const string ImageCacheItemImage = "{1}{0}.jpg";

        public ImageRepository(IStorageService storageService)
        {
            _storageService = storageService.Local;
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
            return await _storageService.OpenFileIfExists(imageFile);
        }

        public async Task DeleteImage(string itemId, string imageId)
        {
            var itemCacheFolder = GetItemCachePath(itemId);

            var imageFile = GetItemImagePath(itemCacheFolder, imageId);
            await _storageService.DeleteFileIfExists(imageFile);
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

            await _storageService.DeleteDirectoryIfExists(itemCacheFolder);
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
            await _storageService.CreateDirectoryIfNotThere("Cache");
            await _storageService.CreateDirectoryIfNotThere(ImageCachePath);

            if (!string.IsNullOrEmpty(path))
            {
                await _storageService.CreateDirectoryIfNotThere(path);
            }
        }
    }
}
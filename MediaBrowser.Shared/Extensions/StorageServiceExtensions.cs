using System.IO;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class StorageServiceExtensions
    {
        public static async Task DeleteDirectoryIfExists(this IAsyncStorageService storageService, string path)
        {
            if (await storageService.DirectoryExistsAsync(path))
            {
                await storageService.DeleteDirectoryAsync(path);
            }
        }

        public static async Task CreateDirectoryIfNotThere(this IAsyncStorageService storageService, string path)
        {
            if (!await storageService.DirectoryExistsAsync(path))
            {
                await storageService.CreateDirectoryAsync(path);
            }
        }

        public static async Task DeleteFileIfExists(this IAsyncStorageService storageService, string file)
        {
            if (await storageService.FileExistsAsync(file))
            {
                await storageService.DeleteFileAsync(file);
            }
        }

        public static async Task<Stream> OpenFileIfExists(this IAsyncStorageService storageService, string file)
        {
            if (await storageService.FileExistsAsync(file))
            {
                return await storageService.OpenFileForReadAsync(file);
            }

            return null;
        }

        public static async Task<string> ReadStringIfFileExists(this IAsyncStorageService storageService, string file)
        {
            if (!await storageService.FileExistsAsync(file))
            {
                return string.Empty;
            }

            return await storageService.ReadAllTextAsync(file);
        }
    }
}

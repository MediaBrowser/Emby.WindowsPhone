using System.IO;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class StorageServiceExtensions
    {
        public static async Task DeleteDirectoryIfExists(this IStorageServiceHandler storageService, string path)
        {
            if (await storageService.DirectoryExistsAsync(path))
            {
                await storageService.DeleteDirectoryAsync(path);
            }
        }

        public static async Task CreateDirectoryIfNotThere(this IStorageServiceHandler storageService, string path)
        {
            if (!await storageService.DirectoryExistsAsync(path))
            {
                await storageService.CreateDirectoryAsync(path);
            }
        }

        public static async Task DeleteFileIfExists(this IStorageServiceHandler storageService, string file)
        {
            if (await storageService.FileExistsAsync(file))
            {
                await storageService.DeleteFileAsync(file);
            }
        }

        public static async Task<Stream> OpenFileIfExists(this IStorageServiceHandler storageService, string file)
        {
            if (await storageService.FileExistsAsync(file))
            {
                return await storageService.OpenFileForReadAsync(file);
            }

            return null;
        }

        public static async Task<string> ReadStringIfFileExists(this IStorageServiceHandler storageService, string file)
        {
            if (!await storageService.FileExistsAsync(file))
            {
                return string.Empty;
            }

            return await storageService.ReadAllTextAsync(file);
        }

        public static Task MoveFileIfExists(this IStorageServiceHandler storageService, string source, string destination)
        {
            return storageService.MoveFileIfExists(source, destination, false);
        }

        public static async Task MoveFileIfExists(this IStorageServiceHandler storageService, string source, string destination, bool overwrite)
        {
            if (await storageService.FileExistsAsync(source))
            {
                var destinationFolder = Path.GetDirectoryName(destination);
                if (!await storageService.DirectoryExistsAsync(destinationFolder))
                {
                    await storageService.CreateDirectoryAsync(destinationFolder);
                }
                await storageService.MoveFileAsync(source, destination, overwrite);
            }
        }
    }
}

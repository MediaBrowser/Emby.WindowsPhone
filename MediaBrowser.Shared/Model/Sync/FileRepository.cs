using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Extensions;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class FileRepository : IFileRepository
    {
        private readonly IStorageServiceHandler _storageService;

        public FileRepository(IStorageService storageService)
        {
            _storageService = storageService.Local;
        }

        public string GetValidFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                if (name.Contains(c))
                    name = name.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty);
            }
            return name;
        }

        public async Task DeleteFile(string path)
        {
            

            await _storageService.DeleteFileIfExists(path);
        }

        public async Task DeleteFolder(string path)
        {
            if (!path.StartsWith("AnyTime"))
            {
                path = AnyTimePath(path);
            }

            await _storageService.DeleteDirectoryIfExists(path);
        }

        public async Task<bool> FileExists(string path)
        {
            try
            {
                if (!path.StartsWith("AnyTime"))
                {
                    path = AnyTimePath(path);
                }

                return await _storageService.FileExistsAsync(path);
            }
            catch
            {
                return false;
            }
        }

        private static string AnyTimePath(string path)
        {
            path = string.Format(Constants.AnyTime.AnyTimeLocation, path);
            return path;
        }

        public async Task<List<DeviceFileInfo>> GetFileSystemEntries(string path)
        {
            var list = new List<DeviceFileInfo>();
            try
            {
                if (!path.StartsWith("AnyTime"))
                {
                    path = AnyTimePath(path);
                }

                if (await _storageService.DirectoryExistsAsync(path))
                {
                    var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(path);
                    if (folder != null)
                    {
                        var files = await folder.GetFilesAsync();
                        list.AddRange(files.Select(f => new DeviceFileInfo {Name = f.Name, Path = string.Concat(path + "\\", f.Name)}));
                    }
                }
            }
            catch
            { }
            return list;
        }

        public string GetFullLocalPath(IEnumerable<string> path)
        {
            var paths = path.ToList();
            return Path.Combine(paths.ToArray());
        }

        public string GetParentDirectoryPath(string path)
        {
            if (!path.StartsWith("AnyTime"))
            {
                path = AnyTimePath(path);
            }

            return Path.GetDirectoryName(path);
        }

        public async Task<Stream> GetFileStream(string path)
        {
            path = AnyTimePath(path); 
            return await _storageService.OpenFileIfExists(path);
        }

        public async Task SaveFile(Stream stream, string path)
        {
            path = AnyTimePath(path); 
            await _storageService.WriteAllBytesAsync(path, await stream.ToArrayAsync());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class FileRepository : IFileRepository
    {
        private readonly IAsyncStorageService _storage;

        public FileRepository(IAsyncStorageService storage)
        {
            _storage = storage;
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
            await _storage.DeleteFileAsync(path);
        }

        public async Task DeleteFolder(string path)
        {
            await _storage.DeleteDirectoryAsync(path);
        }

        public async Task<bool> FileExists(string path)
        {
            try
            {
                return await _storage.FileExistsAsync(path);
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<DeviceFileInfo>> GetFileSystemEntries(string path)
        {
            var list = new List<DeviceFileInfo>();
            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                if (folder != null)
                {
                    var files = await folder.GetFilesAsync();
                    list.AddRange(files.Select(f => new DeviceFileInfo { Name = f.Name, Path = f.Path }));
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
            return Path.GetDirectoryName(path);
        }

        public async Task SaveFile(Stream stream, string path)
        {
            await _storage.WriteAllBytesAsync(path, await stream.ToArrayAsync());
        }
    }
}
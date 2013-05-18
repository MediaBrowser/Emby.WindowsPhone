using Cimbalino.Phone.Toolkit.Services;

namespace MediaBrowser.WindowsPhone.Model
{
    public class ApplicationSettingsDesignService : IApplicationSettingsService
    {
        public T Get<T>(string key)
        {
            return default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            return defaultValue;
        }

        public void Set<T>(string key, T value)
        {
        }

        public void Reset(string key)
        {
        }

        public void Save()
        {
        }

        public void Refresh()
        {
            
        }

        public bool IsDirty { get; private set; }
    }

    public class StorageDesignService: IStorageService
    {

        public void AppendAllLines(string path, System.Collections.Generic.IEnumerable<string> contents, System.Text.Encoding encoding)
        {
        }

        public void AppendAllLines(string path, System.Collections.Generic.IEnumerable<string> contents)
        {
        }

        public void AppendAllText(string path, string contents, System.Text.Encoding encoding)
        {
        }

        public void AppendAllText(string path, string contents)
        {
        }

        public long AvailableFreeSpace
        {
            get { return default(long); }
        }

        public void CopyFile(string sourceFileName, string destinationFileName, bool overwrite)
        {
        }

        public void CopyFile(string sourceFileName, string destinationFileName)
        {
        }

        public void CreateDirectory(string dir)
        {
        }

        public System.IO.Stream CreateFile(string path)
        {
            return null;
        }

        public void DeleteDirectory(string dir)
        {
        }

        public void DeleteFile(string path)
        {
        }

        public bool DirectoryExists(string dir)
        {
            return false;
        }

        public bool FileExists(string path)
        {
            return false;
        }

        public string[] GetDirectoryNames(string searchPattern)
        {
            return new string[0];
        }

        public string[] GetDirectoryNames()
        {
            return new string[0];
        }

        public string[] GetFileNames(string searchPattern)
        {
            return new string[0];
        }

        public string[] GetFileNames()
        {
            return new string[0];
        }

        public bool IncreaseQuotaTo(long newQuotaSize)
        {
            return false;
        }

        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName)
        {
        }

        public void MoveFile(string sourceFileName, string destinationFileName)
        {
        }

        public System.IO.Stream OpenFile(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share)
        {
            return null;
        }

        public System.IO.Stream OpenFile(string path, System.IO.FileMode mode, System.IO.FileAccess access)
        {
            return null;
        }

        public System.IO.Stream OpenFile(string path, System.IO.FileMode mode)
        {
            return null;
        }

        public long Quota
        {
            get { return default(long); }
        }

        public byte[] ReadAllBytes(string path)
        {
            return new byte[0];
        }

        public string[] ReadAllLines(string path, System.Text.Encoding encoding)
        {
            return new string[0];
        }

        public string[] ReadAllLines(string path)
        {
            return new string[0];
        }

        public string ReadAllText(string path, System.Text.Encoding encoding)
        {
            return null;
        }

        public string ReadAllText(string path)
        {
            return null;
        }

        public System.Collections.Generic.IEnumerable<string> ReadLines(string path, System.Text.Encoding encoding)
        {
            return new string[0];
        }

        public System.Collections.Generic.IEnumerable<string> ReadLines(string path)
        {
            return new string[0];
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
        }

        public void WriteAllLines(string path, System.Collections.Generic.IEnumerable<string> contents, System.Text.Encoding encoding)
        {
        }

        public void WriteAllLines(string path, System.Collections.Generic.IEnumerable<string> contents)
        {
        }

        public void WriteAllText(string path, string contents, System.Text.Encoding encoding)
        {
        }

        public void WriteAllText(string path, string contents)
        {
        }
    }
}

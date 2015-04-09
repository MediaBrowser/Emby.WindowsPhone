using Cimbalino.Toolkit.Services;

namespace Emby.WindowsPhone.Design
{
    public class StorageDesignService : IStorageService
    {
        public IStorageServiceHandler Local { get; private set; }
        public IStorageServiceHandler Roaming { get; private set; }
        public IStorageServiceHandler Temporary { get; private set; }
        public IStorageServiceHandler LocalCache { get; private set; }
        public IStorageServiceHandler Package { get; private set; }
    }
}
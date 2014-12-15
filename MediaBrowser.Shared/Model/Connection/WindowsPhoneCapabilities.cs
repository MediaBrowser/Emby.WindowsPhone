using MediaBrowser.Model.Session;

namespace MediaBrowser.WindowsPhone.Model.Connection
{
    public static class WindowsPhoneCapabilities
    {
        public static ClientCapabilities Audio
        {
            get { return CreateCapabilities(false, false, false); }
        }

        public static ClientCapabilities Photos
        {
            get { return CreateCapabilities(false, true, false); }
        }

        public static ClientCapabilities App
        {
            get { return CreateCapabilities(false, true, true); }
        }

        private static ClientCapabilities CreateCapabilities(bool supportsPlayback, bool supportsContentUpload, bool supportsSync)
        {
            return new ClientCapabilities
            {
                SupportsMediaControl = supportsPlayback,
                SupportsContentUploading = supportsContentUpload,
                SupportsSync = supportsSync
            };
        }
    }
}

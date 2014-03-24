using Cimbalino.Phone.Toolkit.Services;

namespace MediaBrowser.WindowsPhone.Design
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
}

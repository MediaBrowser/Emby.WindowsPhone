using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;

namespace Emby.WindowsPhone.Design
{
    public class ApplicationSettingsDesignService : IApplicationSettingsServiceHandler
    {
        public bool Contains(string key)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(string key, T defaultValue)
        {
            throw new System.NotImplementedException();
        }

        public void Set<T>(string key, T value)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<KeyValuePair<string, object>>> GetValuesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}

using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.ApiClient;
using Newtonsoft.Json;

namespace MediaBrowser.WindowsPhone.Model.Connection
{
    public class CredentialProvider : ICredentialProvider
    {
        private readonly IAsyncStorageService _storageService = new AsyncStorageService();

        public async Task<ServerCredentials> GetServerCredentials()
        {
            if (!await _storageService.FileExistsAsync(Constants.Settings.ServerCredentialSettings))
            {
                return new ServerCredentials();
            }

            var json = await _storageService.ReadAllTextAsync(Constants.Settings.ServerCredentialSettings);

            var credentials = JsonConvert.DeserializeObject<ServerCredentials>(json);

            return credentials ?? new ServerCredentials();
        }

        private object lockObject = new object();

        public async Task SaveServerCredentials(ServerCredentials configuration)
        {
            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                lock (lockObject)
                {
                    var json = JsonConvert.SerializeObject(configuration);

                    _storageService.WriteAllTextAsync(Constants.Settings.ServerCredentialSettings, json).ConfigureAwait(false);
                }
            });
        }
    }
}
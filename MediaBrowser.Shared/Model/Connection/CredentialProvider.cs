using System.Threading.Tasks;
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
            var json = await _storageService.ReadAllTextAsync(Constants.Settings.ServerCredentialSettings);

            var credentials = JsonConvert.DeserializeObject<ServerCredentials>(json);

            return credentials ?? new ServerCredentials();
        }

        public async Task SaveServerCredentials(ServerCredentials configuration)
        {
            var json = JsonConvert.SerializeObject(configuration);

            await _storageService.WriteAllTextAsync(Constants.Settings.ServerCredentialSettings, json);
        }
    }
}
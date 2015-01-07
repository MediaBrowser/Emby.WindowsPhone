using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Services
{
    public class SyncService
    {
        private readonly IConnectionManager _connectionManager;
        public static SyncService Current { get; private set; }

        public SyncService(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            Current = this;
        }

        public Task AddJobAsync(string id)
        {
            return AddJobAsync(new List<string> {id});
        }

        public async Task AddJobAsync(List<string> itemIds)
        {
            var request = new SyncJobRequest
            {
                ItemIds = itemIds,
                UserId = AuthenticationService.Current.LoggedInUserId,
                TargetId = _connectionManager.CurrentApiClient.DeviceId
            };

            try
            {
                var job = await _connectionManager.CurrentApiClient.CreateSyncJob(request);
            }
            catch (HttpException ex)
            {
                
            }
        }

        private bool RequiresMoreSpace(float requestedSpace)
        {

            return false;
        }
    }
}

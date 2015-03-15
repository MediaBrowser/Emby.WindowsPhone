using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Services
{
    public class SyncService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly MultiServerSync _mediaSync;
        public static SyncService Current { get; private set; }

        public SyncService(IConnectionManager connectionManager, MultiServerSync mediaSync)
        {
            _connectionManager = connectionManager;
            _mediaSync = mediaSync;
            Current = this;
        }

        public Task AddJobAsync(string id)
        {
            return AddJobAsync(new List<string> { id });
        }

        public async Task AddJobAsync(List<string> itemIds)
        {
            var request = new SyncJobRequest
            {
                ItemIds = itemIds,
                UserId = AuthenticationService.Current.LoggedInUserId,
                TargetId = _connectionManager.CurrentApiClient.DeviceId
            };

            var job = await _connectionManager.CurrentApiClient.CreateSyncJob(request);
        }

        public async Task<List<SyncJob>>  GetSyncJobs()
        {
            var query = new SyncJobQuery
            {
                UserId = AuthenticationService.Current.LoggedInUserId
            };
            var jobs = await _connectionManager.CurrentApiClient.GetSyncJobs(query);

            return jobs != null && !jobs.Items.IsNullOrEmpty() ? jobs.Items.ToList() : new List<SyncJob>();
        }

        public Task Sync()
        {
            return _mediaSync.Sync(new Progress<double>());
        }

        private bool RequiresMoreSpace(float requestedSpace)
        {

            return false;
        }
    }
}

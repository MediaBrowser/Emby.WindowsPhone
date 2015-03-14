using System;
using System.Collections.Generic;
using System.Threading;
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

        public async Task Sync()
        {
            //return _mediaSync.Sync(_connectionManager.CurrentApiClient, App.ServerInfo, new Progress<double>(), default(CancellationToken))
        }

        private bool RequiresMoreSpace(float requestedSpace)
        {

            return false;
        }
    }
}

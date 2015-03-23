using System;
using System.Collections.Generic;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Interfaces;
using MediaBrowser.WindowsPhone.Services;

namespace MediaBrowser.WindowsPhone.Helpers
{
    public class SyncRequestHelper
    {
        private static IConnectionManager _connectionManager;
        private static IServerInfoService _serverInfo;

        public SyncRequestHelper(IConnectionManager connectionManager, IServerInfoService serverInfo)
        {
            _connectionManager = connectionManager;
            _serverInfo = serverInfo;
        }

        public static SyncJobRequest CreateRequest(List<string> itemIds, string name = null)
        {
            var apiClient = _connectionManager.GetApiClient(_serverInfo.ServerInfo.Id);
            var request = new SyncJobRequest
            {
                ItemIds = itemIds,
                UserId = AuthenticationService.Current.LoggedInUserId,
                TargetId = apiClient.DeviceId,
                Name = !string.IsNullOrEmpty(name) ? name : Guid.NewGuid().ToString(),
            };

            return request;
        }

        public static SyncJobRequest CreateRequest(string itemId, string name = null)
        {
            return CreateRequest(new List<string> {itemId}, name);
        }
    }
}

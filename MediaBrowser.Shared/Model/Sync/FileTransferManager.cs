using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Cimbalino.Toolkit.Helpers;
using Cimbalino.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Extensions;
using Microsoft.Phone.BackgroundTransfer;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class FileTransferManager : IFileTransferManager
    {
        private readonly ILocalAssetManager _localAssetManager;
        private readonly IStorageServiceHandler _storageService;

        public FileTransferManager(ILocalAssetManager localAssetManager, IStorageService storageService)
        {
            _localAssetManager = localAssetManager;
            _storageService = storageService.Local;
        }

        public async Task GetItemFileAsync(
            IApiClient apiClient, 
            ServerInfo server, 
            LocalItem item, 
            string syncJobItemId, 
            IProgress<double> transferProgress, 
            CancellationToken cancellationToken)
        {
            var downloadUrl = apiClient.GetSyncJobItemFileUrl(syncJobItemId);

            await CreateDownload(downloadUrl, apiClient, null, "");
        }

        private async Task<BackgroundTransferRequest> CreateDownload(string source, IApiClient client, StorageFile destiantionFile, string notificationName)
        {
            await _storageService.CreateDirectoryIfNotThere("Shared\\Transfers");
            var stringVersion = ApplicationManifest.Current.App.Version;

            var downloader = new BackgroundTransferRequest(new Uri(source, UriKind.Absolute));
            downloader.Headers.Add("X-MediaBrowser-Token", client.AccessToken);
            var authorization = string.Format("MediaBrowser UserId=\"{0}\", Client=\"{1}\", Device=\"{2}\", DeviceId=\"{3}\", Version=\"{4}\"", client.CurrentUserId, client.ClientName, client.DeviceName, client.DeviceId, stringVersion);
            downloader.Headers.Add("Authorization", authorization);
            downloader.Method = "GET";

            var downloadLocation = new Uri("Shared\\transfers\\sync", UriKind.RelativeOrAbsolute);

            return downloader;
        }
    }
}
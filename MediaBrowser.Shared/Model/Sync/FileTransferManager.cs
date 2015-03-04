using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
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
        private readonly IAsyncStorageService _storageService;

        public FileTransferManager(ILocalAssetManager localAssetManager, IAsyncStorageService storageService)
        {
            _localAssetManager = localAssetManager;
            _storageService = storageService;
        }

        public Task GetItemFileAsync(IApiClient apiClient, ServerInfo server, LocalItem item, string syncJobItemId, IProgress<double> transferProgress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<BackgroundTransferRequest> CreateDownload(string source, IApiClient client, StorageFile destiantionFile, string notificationName)
        {
            await _storageService.CreateDirectoryIfNotThere("/shared/transfers");
            var stringVersion = ApplicationManifest.Current.App.Version;

            var downloader = new BackgroundTransferRequest(new Uri(""));
            downloader.Headers.Add("X-MediaBrowser-Token", client.AccessToken);
            var authorization = string.Format("MediaBrowser UserId=\"{0}\", Client=\"{1}\", Device=\"{2}\", DeviceId=\"{3}\", Version=\"{4}\"", client.CurrentUserId, client.ClientName, client.DeviceName, client.DeviceId, stringVersion);
            downloader.Headers.Add("Authorization", authorization);
            downloader.Method = "GET";

            var downloadLocation = new Uri("shared/transfers/sync/", UriKind.RelativeOrAbsolute);

            //TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310BlockAndText01);
            //downloader.SuccessToastNotification = Notification.NotificationManager.CreateToast(Shared.Globalization.Strings.SyncJobItemStatusSynced, notificationName.Replace("&", "&amp;"));
            //downloader.FailureToastNotification = Notification.NotificationManager.CreateToast(Shared.Globalization.Strings.SyncJobItemStatusFailed, notificationName.Replace("&", "&amp;"));

            return downloader;
        }
    }
}
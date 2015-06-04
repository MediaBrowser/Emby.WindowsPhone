using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Interfaces;
using Emby.WindowsPhone.Model.Sync;
using Microsoft.Phone.BackgroundTransfer;
using Newtonsoft.Json;
using ScottIsAFool.WindowsPhone.Extensions;
using ScottIsAFool.WindowsPhone.Logging;

namespace Emby.WindowsPhone.Services
{
    public class SyncService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IMultiServerSync _mediaSync;
        private readonly IServerInfoService _serverInfo;
        private readonly IMessagePromptService _messagePrompt;
        private readonly IMessengerService _messengerService;
        private readonly IStorageServiceHandler _storageService;
        private readonly ILog _logger;

        public static SyncService Current { get; private set; }

        public SyncService(
            IConnectionManager connectionManager,
            IMultiServerSync mediaSync,
            IStorageService storageService,
            IServerInfoService serverInfo,
            IMessagePromptService messagePrompt,
            IMessengerService messengerService)
        {
            _connectionManager = connectionManager;
            _mediaSync = mediaSync;
            _serverInfo = serverInfo;
            _messagePrompt = messagePrompt;
            _messengerService = messengerService;
            _storageService = storageService.Local;
            _logger = new WPLogger(GetType());
            Current = this;
        }

        public Task StartService()
        {
            Sync().ConfigureAwait(false);
            CheckAndMoveFinishedFiles().ConfigureAwait(false);

            ListenForEvents();

            return Task.FromResult(0);
        }

        private void ListenForEvents()
        {
            _connectionManager.Connected += ConnectionManagerOnConnected;

            if (_serverInfo.HasServer)
            {
                var apiClient = _connectionManager.GetApiClient(_serverInfo.ServerInfo.Id);
                WireUpApiClient(apiClient);
            }
        }

        private void ConnectionManagerOnConnected(object sender, GenericEventArgs<ConnectionResult> e)
        {
            var apiClient = e.Argument.ApiClient;
            WireUpApiClient(apiClient);
        }

        private void WireUpApiClient(IApiClient apiClient)
        {
            apiClient.SyncJobCreated -= ApiClientOnSyncJobCreated;
            apiClient.SyncJobCreated += ApiClientOnSyncJobCreated;

            apiClient.SyncJobCancelled -= ApiClientOnSyncJobCancelled;
            apiClient.SyncJobCancelled += ApiClientOnSyncJobCancelled;

            apiClient.SyncJobUpdated -= ApiClientOnSyncJobUpdated;
            apiClient.SyncJobUpdated += ApiClientOnSyncJobUpdated;

            apiClient.SyncJobsUpdated -= ApiClientOnSyncJobsUpdated;
            apiClient.SyncJobsUpdated += ApiClientOnSyncJobsUpdated;
        }

        private void ApiClientOnSyncJobsUpdated(object sender, GenericEventArgs<List<SyncJob>> genericEventArgs)
        {
            Sync().ConfigureAwait(false);
        }

        private void ApiClientOnSyncJobUpdated(object sender, GenericEventArgs<CompleteSyncJobInfo> genericEventArgs)
        {
            Sync().ConfigureAwait(false);
        }

        private void ApiClientOnSyncJobCancelled(object sender, GenericEventArgs<SyncJob> genericEventArgs)
        {
            Sync().ConfigureAwait(false);
        }

        private void ApiClientOnSyncJobCreated(object sender, GenericEventArgs<SyncJobCreationResult> genericEventArgs)
        {
            Sync().ConfigureAwait(false);
        }

        public async Task AddJobAsync(SyncJobRequest request)
        {
            var apiClient = _connectionManager.GetApiClient(_serverInfo.ServerInfo.Id);
            var qualityOptions = await _messagePrompt.RequestSyncOption(request);

            if (qualityOptions != null)
            {
                _logger.Info("Quality requested for {0} is {1}", request.Name, qualityOptions.Quality.Name);
                request.Quality = qualityOptions.Quality.Name;
                request.ItemLimit = qualityOptions.ItemLimit;
                request.SyncNewContent = qualityOptions.AutoSyncNewItems;
                request.UnwatchedOnly = qualityOptions.UnwatchedItems;
                request.Profile = qualityOptions.Profile != null ? qualityOptions.Profile.Id : string.Empty;

                _logger.Info("Create sync job");
                var job = await apiClient.CreateSyncJob(request);
                if (job != null)
                {
                    _messagePrompt.ShowMessage(AppResources.MessageSyncJobCreated);
                    _logger.Info("Job created, start sync request");
                    await Sync().ConfigureAwait(false);
                }
            }
            else
            {
                _logger.Info("No quality given by the user, most likely dismissed (back button)");
            }
        }

        public async Task<List<SyncJob>> GetSyncJobs(bool onlyThisDevice = true)
        {
            var apiClient = _connectionManager.GetApiClient(_serverInfo.ServerInfo.Id);
            var query = new SyncJobQuery
            {
                UserId = AuthenticationService.Current.LoggedInUserId,
                TargetId = onlyThisDevice ? apiClient.DeviceId : string.Empty
            };

            var jobs = await apiClient.GetSyncJobs(query);

            if (jobs != null && !jobs.Items.IsNullOrEmpty())
            {
                return jobs.Items.ToList();
            }

            return new List<SyncJob>();
        }

        public Task UnsyncItem(string id)
        {
            return UnsyncItems(new List<string> { id });
        }

        public async Task UnsyncItems(IEnumerable<string> itemIds)
        {
            var apiClient = _connectionManager.GetApiClient(_serverInfo.ServerInfo.Id);
            await apiClient.CancelSyncLibraryItems(apiClient.DeviceId, itemIds);

            Sync().ConfigureAwait(false);
        }

        public Task CheckAndMoveFinishedFiles()
        {
            var completedTasks = BackgroundTransferService.Requests.Where(x => x.TransferStatus == TransferStatus.Completed).ToList();
            var list = completedTasks.Select(MoveCompleted).ToList();

            return Task.WhenAll(list).ContinueWith(task => completedTasks = null);
        }

        public async Task MoveCompleted(BackgroundTransferRequest request)
        {
            var item = JsonConvert.DeserializeObject<JobData>(request.Tag);
            var finalFile = item.Location;
            var downloadLocation = string.Format(Constants.AnyTime.DownloadLocation, item.Id);

            await _storageService.MoveFileIfExists(downloadLocation, finalFile, true);
            await _storageService.DeleteFileIfExists(downloadLocation);

            BackgroundTransferService.Remove(request);

            _messengerService.SendSyncNotification(Constants.Messages.SyncJobFinishedMsg, item.Id, item.ItemType);
        }

        public Task Sync()
        {
            if (!AuthenticationService.Current.IsLoggedIn
                || !AuthenticationService.Current.LoggedInUser.Policy.EnableSync)
            {
                return Task.FromResult(0);
            }

            try
            {
                return _mediaSync.Sync(new Progress<double>());
            }
            catch (HttpException ex)
            {

            }

            return Task.FromResult(0);
        }

        private bool RequiresMoreSpace(float requestedSpace)
        {

            return false;
        }
    }
}

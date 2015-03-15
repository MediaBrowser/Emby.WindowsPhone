﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Model.Sync;
using Microsoft.Phone.BackgroundTransfer;
using Newtonsoft.Json;
using ScottIsAFool.WindowsPhone.Extensions;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Services
{
    public class SyncService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IMultiServerSync _mediaSync;
        private readonly IStorageServiceHandler _storageService;
        private readonly ILog _logger;
        public static SyncService Current { get; private set; }

        public SyncService(IConnectionManager connectionManager, IMultiServerSync mediaSync, IStorageService storageService)
        {
            _connectionManager = connectionManager;
            _mediaSync = mediaSync;
            _storageService = storageService.Local;
            _logger = new WPLogger(GetType());
            Current = this;
        }

        public Task StartService()
        {
            Sync().ConfigureAwait(false);
            CheckAndMoveFinishedFiles().ConfigureAwait(false);

            return Task.FromResult(0);
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

        public async Task<List<SyncJob>> GetSyncJobs()
        {
            var query = new SyncJobQuery
            {
                UserId = AuthenticationService.Current.LoggedInUserId
            };
            var jobs = await _connectionManager.CurrentApiClient.GetSyncJobs(query);

            return jobs != null && !jobs.Items.IsNullOrEmpty() ? jobs.Items.ToList() : new List<SyncJob>();
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
            var finalFile = string.Format(Constants.AnyTime.AnyTimeLocation, item.Location);
            var downloadLocation = string.Format(Constants.AnyTime.DownloadLocation, item.Id);

            await _storageService.MoveFileIfExists(downloadLocation, finalFile, true);
            await _storageService.DeleteFileIfExists(downloadLocation);
        }

        public Task Sync()
        {
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

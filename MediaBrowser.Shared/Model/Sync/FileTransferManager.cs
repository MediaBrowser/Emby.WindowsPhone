using System;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class FileTransferManager : IFileTransferManager
    {
        public Task GetItemFileAsync(IApiClient apiClient, ServerInfo server, LocalItem item, string syncJobItemId, IProgress<double> transferProgress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
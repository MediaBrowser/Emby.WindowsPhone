using System;
using System.Threading.Tasks;
using MediaBrowser.Model.Sync;
using Emby.WindowsPhone.Model.Sync;

namespace Emby.WindowsPhone.Interfaces
{
    public interface IMessagePromptService
    {
        Task<SyncOption> RequestSyncOption(SyncJobRequest request);
        void ShowMessage(string message, string title = "", Action action = null);
    }
}
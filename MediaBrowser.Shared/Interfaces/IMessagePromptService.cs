using System.Threading.Tasks;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Model.Sync;

namespace MediaBrowser.WindowsPhone.Interfaces
{
    public interface IMessagePromptService
    {
        Task<SyncOption> RequestSyncOption(SyncJobRequest request);
    }
}
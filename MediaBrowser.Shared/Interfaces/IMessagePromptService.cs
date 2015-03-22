using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Interfaces
{
    public interface IMessagePromptService
    {
        Task<SyncQualityOption> RequestSyncOption(List<SyncQualityOption> options);
    }
}
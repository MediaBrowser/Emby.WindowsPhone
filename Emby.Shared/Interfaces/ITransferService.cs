using Microsoft.Phone.BackgroundTransfer;

namespace Emby.WindowsPhone.Interfaces
{
    public interface ITransferService
    {
        void Add(BackgroundTransferRequest request);
    }
}
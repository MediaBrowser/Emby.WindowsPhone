using Emby.WindowsPhone.Interfaces;
using Microsoft.Phone.BackgroundTransfer;

namespace Emby.WindowsPhone.Services
{
    public class TransferService : ITransferService
    {
        public void Add(BackgroundTransferRequest request)
        {
            BackgroundTransferService.Add(request);
        }
    }
}

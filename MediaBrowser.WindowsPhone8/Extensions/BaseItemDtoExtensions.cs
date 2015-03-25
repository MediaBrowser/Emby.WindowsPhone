using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Resources;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class BaseItemDtoExtensions
    {
        public static bool CanTakeOffline(this BaseItemDto item)
        {
            if (!item.SupportsSync.HasValue || !item.SupportsSync.Value)
            {
                App.ShowMessage(AppResources.ErrorNoSyncSupport);
                return false;
            }

            return !item.HasSyncJob.HasValue || !item.HasSyncJob.Value;
        }
    }
}

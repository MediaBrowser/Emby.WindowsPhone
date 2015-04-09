using MediaBrowser.Model.Dto;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.Model.Sync;

namespace Emby.WindowsPhone.Extensions
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

        public static string EpisodeString(this BaseItemDto item)
        {
            return string.Format("{0} - {1}x{2} - {3}", item.SeriesName, item.ParentIndexNumber, item.IndexNumber, item.Name);
        }

        public static string SeasonString(this BaseItemDto item)
        {
            return string.Format("{0} - {1}", item.SeriesName, item.Name);
        }

        public static string GetTvTypeName(this BaseItemDto item)
        {
            switch (item.Type.ToLower())
            {
                case "episode":
                    return item.EpisodeString();
                case "season":
                    return item.SeasonString();
                default:
                    return item.Name;
            }
        }
    }

    public static class SyncProfileOptionExtensions
    {
        public static string GetName(this SyncProfileOption option)
        {
            var id = string.Format("Profile{0}", option.Name);
            var name = AppResources.ResourceManager.GetString(id);
            return string.IsNullOrEmpty(name) ? option.Name : name;
        }

        public static string GetDescription(this SyncProfileOption option)
        {
            var id = string.Format("Profile{0}Description", option.Name);
            var description = AppResources.ResourceManager.GetString(id);
            return string.IsNullOrEmpty(description) ? option.Name : description;
        }
    }
}

using MediaBrowser.Model.Dto;
using Emby.WindowsPhone.Localisation;

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

        public static void HasPlayed(this BaseItemDto item)
        {
            if (item == null || !item.RunTimeTicks.HasValue)
            {
                return;
            }

            var runtime = item.RunTimeTicks.Value;
            var watched = item.UserData.PlaybackPositionTicks;

            var pct = ((double)watched/(double)runtime)*100;

            if (watched == 0 || pct >= 90)
            {
                item.UserData.Played = true;
            }
        }
    }
}

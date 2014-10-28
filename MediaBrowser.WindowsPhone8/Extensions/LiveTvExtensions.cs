using System.Linq;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.WindowsPhone.Services;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class LiveTvExtensions
    {
        public static bool UserCanHasLiveTv(this LiveTvInfo liveTvInfo, string userId)
        {
            var tvInfo = liveTvInfo;
            if (tvInfo == null || tvInfo.EnabledUsers.IsNullOrEmpty())
            {
                return false;
            }

            var allowedUser = tvInfo.EnabledUsers.FirstOrDefault(x => x == AuthenticationService.Current.LoggedInUserId);

            return allowedUser != null;
        }
    }
}

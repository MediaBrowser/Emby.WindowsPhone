using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone
{
    public static class LiveTvUtils
    {
        public static async Task CreateSeriesLink(ProgramInfoDto item, IExtendedApiClient apiClient, INavigationService navigationService, ILog log)
        {
            try
            {
                var timer = await apiClient.GetDefaultLiveTvTimerInfo(item.Id, default(CancellationToken));

                if (timer != null)
                {
                    await apiClient.CreateLiveTvSeriesTimerAsync(timer, default(CancellationToken));
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "RecordProgrammeCommand", navigationService, log);
            }
        }

        public static async Task RecordProgramme(ProgramInfoDto item, IExtendedApiClient apiClient, INavigationService navigationService, ILog log)
        {
            try
            {
                var timer = await apiClient.GetDefaultLiveTvTimerInfo(item.Id, default(CancellationToken));

                if (timer != null)
                {
                    //await _apiClient.CreateLiveTvTimerAsync(timer, default(CancellationToken));
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "RecordProgrammeCommand", navigationService, log);
            }
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone
{
    public static class LiveTvUtils
    {
        public static async Task<string> CreateSeriesLink(ProgramInfoDto item, IExtendedApiClient apiClient, INavigationService navigationService, ILog log)
        {
            try
            {
                var timer = await apiClient.GetDefaultLiveTvTimerInfo(item.Id, default(CancellationToken));

                if (timer != null)
                {
                    await apiClient.CreateLiveTvSeriesTimerAsync(timer, default(CancellationToken));

                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.NewSeriesRecordingAddedMsg));

                    return timer.Id;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "RecordProgrammeCommand", navigationService, log);
            }

            return null;
        }

        public static async Task<string> RecordProgramme(ProgramInfoDto item, IExtendedApiClient apiClient, INavigationService navigationService, ILog log)
        {
            try
            {
                var timer = await apiClient.GetDefaultLiveTvTimerInfo(item.Id, default(CancellationToken));

                if (timer != null)
                {
                    //await _apiClient.CreateLiveTvTimerAsync(timer, default(CancellationToken));

                    return timer.Id;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "RecordProgrammeCommand", navigationService, log);
            }

            return null;
        }

        public static async Task CancelSeries(SeriesTimerInfoDto selectedSeries, INavigationService navigationService, IExtendedApiClient apiClient, ILog log, bool goBack)
        {
            try
            {
                await apiClient.CancelLiveTvSeriesTimerAsync(selectedSeries.Id, default(CancellationToken));

                Messenger.Default.Send(new NotificationMessage(selectedSeries.Id, Constants.Messages.LiveTvSeriesDeletedMsg));

                if (navigationService.CanGoBack && goBack)
                {
                    navigationService.GoBack();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "CancelSeriesRecording", navigationService, log);
                MessageBox.Show(AppResources.ErrorDeletingSeriesRecording, AppResources.ErrorTitle, MessageBoxButton.OK);
            }
        }

        public static async Task CancelRecording(TimerInfoDto item, INavigationService navigationService, IExtendedApiClient apiClient, ILog log, bool goBack = false)
        {
            try
            {
                await apiClient.CancelLiveTvTimerAsync(item.Id, default(CancellationToken));

                if (navigationService.CanGoBack && goBack)
                {
                    navigationService.GoBack();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "CancelRecordingCommand", navigationService, log);
            }
        }
    }
}

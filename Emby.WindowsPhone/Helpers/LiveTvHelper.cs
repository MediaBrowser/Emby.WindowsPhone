using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.Model.Dto;
using ScottIsAFool.WindowsPhone.Logging;

namespace Emby.WindowsPhone.Helpers
{
    public static class LiveTvHelper
    {
        public static async Task<string> CreateSeriesLink(BaseItemDto item, IApiClient apiClient, INavigationService navigationService, ILog log)
        {
            try
            {
                var timer = await apiClient.GetDefaultLiveTvTimerInfo(item.Id);

                if (timer != null)
                {
                    await apiClient.CreateLiveTvSeriesTimerAsync(timer);

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

        public static async Task<string> RecordProgramme(BaseItemDto item, IApiClient apiClient, INavigationService navigationService, ILog log)
        {
            try
            {
                var timer = await apiClient.GetDefaultLiveTvTimerInfo(item.Id);

                if (timer != null)
                {
                    await apiClient.CreateLiveTvTimerAsync(timer);

                    return timer.Id;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "RecordProgrammeCommand", navigationService, log);
            }

            return null;
        }

        public static async Task<bool> CancelSeries(SeriesTimerInfoDto selectedSeries, INavigationService navigationService, IApiClient apiClient, ILog log, bool goBack)
        {
            return await CancelSeries(selectedSeries.Id, navigationService, apiClient, log, goBack);
        }

        public static async Task<bool> CancelSeries(string seriesId, INavigationService navigationService, IApiClient apiClient, ILog log, bool goBack)
        {
            try
            {
                await apiClient.CancelLiveTvSeriesTimerAsync(seriesId);

                Messenger.Default.Send(new NotificationMessage(seriesId, Constants.Messages.LiveTvSeriesDeletedMsg));

                if (navigationService.CanGoBack && goBack)
                {
                    navigationService.GoBack();
                }

                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "CancelSeriesRecording", navigationService, log);
                MessageBox.Show(AppResources.ErrorDeletingSeriesRecording, AppResources.ErrorTitle, MessageBoxButton.OK);
            }

            return false;
        }

        public static async Task<bool> CancelRecording(TimerInfoDto item, INavigationService navigationService, IApiClient apiClient, ILog log, bool goBack = false)
        {
            return await CancelRecording(item.Id, navigationService, apiClient, log, goBack);
        }

        public static async Task<bool> CancelRecording(string itemId, INavigationService navigationService, IApiClient apiClient, ILog log, bool goBack = false)
        {
            try
            {
                await apiClient.CancelLiveTvTimerAsync(itemId);

                if (navigationService.CanGoBack && goBack)
                {
                    navigationService.GoBack();
                }

                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "CancelRecordingCommand", navigationService, log);
            }

            return false;
        }

        public static bool HasExpired(DateTime? lastRun, int timeOutMinutes = 30)
        {
            if (!lastRun.HasValue)
            {
                return true;
            }

            var difference = DateTime.Now - lastRun.Value;
            return difference.TotalMinutes > timeOutMinutes;
        }
    }
}

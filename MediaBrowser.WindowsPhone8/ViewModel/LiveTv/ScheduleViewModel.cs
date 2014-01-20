using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Reactive;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScheduleViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _seriesLoaded;
        private bool _upcomingLoaded;

        /// <summary>
        /// Initializes a new instance of the ScheduleViewModel class.
        /// </summary>
        public ScheduleViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public int SelectedIndex { get; set; }
        public List<SeriesTimerInfoDto> Series { get; set; }
        public List<TimerInfoDto> Upcoming { get; set; }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }

        public RelayCommand SchedulePageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetScheduledSeriesRecordings(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetData(true);
                });
            }
        }

        [UsedImplicitly]
        private async void OnSelectedIndexChanged()
        {
            await GetData(false);
        }

        private async Task GetData(bool isRefresh)
        {
            switch (SelectedIndex)
            {
                case 0:
                    await GetScheduledSeriesRecordings(isRefresh);
                    break;
                case 1:
                    await GetScheduledUpcomingRecordings(isRefresh);
                    break;
            }
        }

        private async Task GetScheduledSeriesRecordings(bool refresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_seriesLoaded && !refresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingSeriesRecordings);

                var query = new SeriesTimerQuery();
                var items = await _apiClient.GetLiveTvSeriesTimersAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Series = items.Items.ToList();
                    _seriesLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetScheduledRecordings()", _navigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetScheduledUpcomingRecordings(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_upcomingLoaded & !isRefresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingUpcomingRecordings);

                var items = await _apiClient.GetLiveTvTimersAsync(new TimerQuery(), default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Upcoming = items.Items.ToList();

                    _upcomingLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetScheduledUpcomingRecordings()", ex, _navigationService, Log);
            }

            SetProgressBar();
        }
    }
}
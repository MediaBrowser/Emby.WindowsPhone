using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LiveTvViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _upcomingLoaded;
        private bool _whatsOnLoaded;

        private DateTime? _upcomingLastRun;
        private DateTime? _whatsOnLastRun;

        /// <summary>
        /// Initializes a new instance of the LiveTvViewModel class.
        /// </summary>
        public LiveTvViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public List<ProgramInfoDto> WhatsOn { get; set; }
        public List<ProgramInfoDto> Upcoming { get; set; }

        public bool ShowMoreWhatsOn { get; set; }

        public bool ShowMoreUpcoming { get; set; }

        public RelayCommand LiveTvPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            await GetWhatsOn(isRefresh);
            await GetUpcoming(isRefresh);
        }

        private async Task GetUpcoming(bool isRefresh)
        {
            if (_upcomingLoaded && !isRefresh && !HasExpired(_upcomingLastRun))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingUpcomingRecordings);

                var query = new RecommendedProgramQuery
                {
                    HasAired = false,
                    Limit = 7,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetRecommendedLiveTvProgramsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Upcoming = items.Items.Take(6).ToList();

                    ShowMoreUpcoming = items.Items.Count() > 6;

                    _upcomingLoaded = true;
                    _upcomingLastRun = DateTime.Now;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetUpcoming()", ex, _navigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetWhatsOn(bool isRefresh)
        {
            if (_whatsOnLoaded && !isRefresh && !HasExpired(_whatsOnLastRun))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SystemTrayGettingWhatsOn);

                var query = new RecommendedProgramQuery
                {
                    IsAiring = true,
                    Limit = 7,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetRecommendedLiveTvProgramsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    WhatsOn = items.Items.Take(6).ToList();

                    ShowMoreWhatsOn = items.Items.Count() > 6;

                    _whatsOnLoaded = true;
                    _whatsOnLastRun = DateTime.Now;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetUpcoming()", ex, _navigationService, Log);
            }

            SetProgressBar();
        }

        public bool HasExpired(DateTime? lastRun)
        {
            if (!lastRun.HasValue)
            {
                return true;
            }

            var difference = DateTime.Now - lastRun.Value;
            return difference.TotalMinutes > 30;
        }
    }
}
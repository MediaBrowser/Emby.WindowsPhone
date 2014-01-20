using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
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

        private bool _scheduleLoaded;

        /// <summary>
        /// Initializes a new instance of the ScheduleViewModel class.
        /// </summary>
        public ScheduleViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

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
                    await GetScheduledRecordings(false);
                });
            }
        }

        private async Task GetScheduledRecordings(bool refresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_scheduleLoaded && !refresh))
            {
                return;
            }

            try
            {
                var query = new SeriesTimerQuery();
                var items = await _apiClient.GetLiveTvSeriesTimersAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetScheduledRecordings()", _navigationService, Log);
            }
        }
    }
}
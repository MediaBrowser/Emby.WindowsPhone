using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
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

        public RelayCommand LiveTvPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    
                });
            }
        }

        private async Task<bool> GetUpcoming()
        {
            try
            {
                var query = new RecommendedProgramQuery
                {
                    HasAired = false,
                    Limit = 6,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetRecommendedLiveTvProgramsAsync(query, default(CancellationToken));

                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetUpcoming()", ex, _navigationService, Log);
            }

            return false;
        }

        private async Task<bool> GetWhatsOn()
        {
            try
            {
                var query = new RecommendedProgramQuery
                {
                    IsAiring = true,
                    Limit = 6,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetRecommendedLiveTvProgramsAsync(query, default(CancellationToken));
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetUpcoming()", ex, _navigationService, Log);
            }

            return false;
        }
    }
}
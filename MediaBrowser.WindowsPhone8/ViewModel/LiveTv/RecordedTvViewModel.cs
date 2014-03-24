using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RecordedTvViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _programmesLoaded;

        /// <summary>
        /// Initializes a new instance of the RecordedTvViewModel class.
        /// </summary>
        public RecordedTvViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public List<RecordingInfoDto> RecordedProgrammes { get; set; }
        public List<Group<RecordingInfoDto>> GroupedRecordedProgrammes { get; set; }

        public DataTemplate GroupHeaderTemplate { get; set; }
        public Style GroupItemTemplate { get; set; }
        public RecordedGroupBy GroupBy { get; set; }

        public RelayCommand RecordedTvViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadProgrammes(false);
                });
            }
        }

        private async Task LoadProgrammes(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_programmesLoaded && !isRefresh))
            {
                return;
            }

            try
            {
                var query = new RecordingQuery
                {
                    IsInProgress = false,
                    Status = RecordingStatus.Completed,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetLiveTvRecordingsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    RecordedProgrammes = items.Items.ToList();
                    await GroupProgrammes();
                }

            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadProgrammes(" + isRefresh + ")", _navigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GroupProgrammes()
        {
            if (RecordedProgrammes.IsNullOrEmpty())
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayRegrouping);

            await Task.Run(() =>
            {
                var emptyGroups = new List<Group<RecordingInfoDto>>();
            });
        }
    }
}
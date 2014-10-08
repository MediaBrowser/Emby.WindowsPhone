using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.Channels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChannelsViewModel : ViewModelBase
    {
        private readonly IExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        private bool _channelsLoaded;

        /// <summary>
        /// Initializes a new instance of the ChannelsViewModel class.
        /// </summary>
        public ChannelsViewModel(IExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
        }

        public ObservableCollection<BaseItemDto> Channels { get; set; }

        public RelayCommand ChannelsViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshChannelsCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable 
                || (_channelsLoaded && !isRefresh))
            {
                return;
            }

            var query = new ChannelQuery
            {
                UserId = AuthenticationService.Current.LoggedInUserId
            };

            try
            {
                SetProgressBar(AppResources.SysTrayGettingChannels);

                var items = await _apiClient.GetChannels(query);

                Channels = new ObservableCollection<BaseItemDto>(items.Items);

                _channelsLoaded = true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", _navigationService, Log);
            }

            SetProgressBar();
        }
    }
}
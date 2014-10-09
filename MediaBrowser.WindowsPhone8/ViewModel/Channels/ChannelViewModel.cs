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
    public class ChannelViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _channelItemsLoaded;

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public ChannelViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public BaseItemDto SelectedChannel { get; set; }

        public ObservableCollection<BaseItemDto> ChannelItems { get; set; }

        public RelayCommand ChannelViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        public RelayCommand<BaseItemDto> ItemTappedCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(item => _navigationService.NavigateTo(item));
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable ||
                SelectedChannel == null ||
                (_channelItemsLoaded && !isRefresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingDetails);

                var query = new ChannelItemQuery
                {
                    ChannelId = SelectedChannel.Type.ToLower() == "channel" ? SelectedChannel.Id : SelectedChannel.ChannelId,
                    FolderId = SelectedChannel.Type.ToLower() == "channel" ? "" : SelectedChannel.Id,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };
                var items = await _apiClient.GetChannelItems(query);

                ChannelItems = new ObservableCollection<BaseItemDto>(items.Items);

                _channelItemsLoaded = true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", _navigationService, Log);
            }

            SetProgressBar();
        }
    }
}
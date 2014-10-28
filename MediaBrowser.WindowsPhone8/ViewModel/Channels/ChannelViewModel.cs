using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;


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
        private bool _channelItemsLoaded;

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public ChannelViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }

        public BaseItemDto SelectedChannel { get; set; }

        public ObservableCollection<BaseItemDto> ChannelItems { get; set; }

        public RelayCommand ChannelViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    ServerIdItem = SelectedChannel;
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
                return new RelayCommand<BaseItemDto>(item => NavigationService.NavigateTo(item));
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!NavigationService.IsNetworkAvailable ||
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
                var items = await ApiClient.GetChannelItems(query);

                ChannelItems = new ObservableCollection<BaseItemDto>(items.Items);

                _channelItemsLoaded = true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", NavigationService, Log);
            }

            SetProgressBar();
        }
    }
}
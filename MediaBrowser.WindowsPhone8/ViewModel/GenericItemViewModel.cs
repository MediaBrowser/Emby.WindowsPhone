using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GenericItemViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the GenericItemViewModel class.
        /// </summary>
        public GenericItemViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public BaseItemDto SelectedItem { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetItemDetails();
                });
            }
        }

        private async Task GetItemDetails()
        {
            if (!_navigationService.IsNetworkAvailable || _dataLoaded || SelectedItem == null)
            {
                return;
            }

            var item = await _apiClient.GetItemAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId);

            SelectedItem = item;
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.GenericItemChangedMsg))
                {
                    SelectedItem = (BaseItemDto) m.Sender;
                    _dataLoaded = false;
                }
            });
        }
    }
}
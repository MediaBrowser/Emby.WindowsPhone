using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Services;

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
        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the GenericItemViewModel class.
        /// </summary>
        public GenericItemViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }

        public BaseItemDto SelectedItem { get; set; }

        [UsedImplicitly]
        private void OnSelectedItemChanged()
        {
            ServerIdItem = SelectedItem;
        }

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
            if (!NavigationService.IsNetworkAvailable || _dataLoaded || SelectedItem == null)
            {
                return;
            }

            var item = await ApiClient.GetItemAsync(SelectedItem.Id, AuthenticationService.Current.LoggedInUserId);

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
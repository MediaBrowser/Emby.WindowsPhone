using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emby.WindowsPhone.Model.Interfaces;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.ViewModel.Offline
{
    public class OfflineUsersViewModel : ViewModelBase
    {
        public OfflineUsersViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }

        public List<UserDto> Users { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadUsers();
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadUsers();
                });
            }
        }

        private async Task LoadUsers()
        {
            var users = await ConnectionManager.GetOfflineUsers();

            if (!users.IsNullOrEmpty())
            {
                Users = users;
            }
        }
    }
}

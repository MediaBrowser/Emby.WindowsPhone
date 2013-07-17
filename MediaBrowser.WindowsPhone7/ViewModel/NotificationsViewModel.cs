using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Notifications;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NotificationsViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the NotificationsViewModel class.
        /// </summary>
        public NotificationsViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            Notifications = new ObservableCollection<Notification>();
        }

        public ObservableCollection<Notification> Notifications { get; set; }

        public RelayCommand NotificationPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (Notifications != null)
                    {
                        Notifications.Clear();
                    }

                    await GetNotifications();
                });
            }
        }

        private async Task GetNotifications()
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            SetProgressBar("Getting notifications...");

            var query = new NotificationQuery
            {
                StartIndex = 0,
                UserId = App.Settings.LoggedInUser.Id
            };

            var notifications = await _apiClient.GetNotificationsAsync(query);
            Notifications = new ObservableCollection<Notification>(notifications.Notifications);

            await _apiClient.MarkNotificationsRead(App.Settings.LoggedInUser.Id, Notifications.Select(x => x.Id), true);

            var summary = await _apiClient.GetNotificationsSummary(App.Settings.LoggedInUser.Id);

            Messenger.Default.Send(new NotificationMessage(summary, Constants.Messages.NotificationCountMsg));

            SetProgressBar();
        }
    }
}
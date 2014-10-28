using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Notifications;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;


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
        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the NotificationsViewModel class.
        /// </summary>
        public NotificationsViewModel(IConnectionManager connectionManager, INavigationService navigationService)
            : base(navigationService, connectionManager)
        {
            Notifications = new ObservableCollection<Notification>();
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.NotifcationNavigationMsg))
                {
                    _dataLoaded = false;
                }
            });
        }

        public ObservableCollection<Notification> Notifications { get; set; }
        public Notification SelectedNotification { get; set; }

        public RelayCommand NotificationPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (Notifications != null && !_dataLoaded)
                    {
                        Notifications.Clear();
                    }

                    await GetNotifications();
                });
            }
        }

        public RelayCommand<Notification> NotificationTappedCommand
        {
            get
            {
                return new RelayCommand<Notification>(notification =>
                {
                    if (notification == null)
                    {
                        return;
                    }

                    SelectedNotification = notification;

                    NavigationService.NavigateTo(Constants.Pages.NotificationView);
                });
            }
        }

        private async Task GetNotifications()
        {
            if (!NavigationService.IsNetworkAvailable || _dataLoaded)
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayGettingNotifications);

            try
            {
                var query = new NotificationQuery
                {
                    StartIndex = 0,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Limit = 20
                };

                var notifications = await ApiClient.GetNotificationsAsync(query);
                Notifications = new ObservableCollection<Notification>(notifications.Notifications);

                await ApiClient.MarkNotificationsRead(AuthenticationService.Current.LoggedInUser.Id, Notifications.Select(x => x.Id), true);

                var summary = await ApiClient.GetNotificationsSummary(AuthenticationService.Current.LoggedInUser.Id);

                Messenger.Default.Send(new NotificationMessage(summary, Constants.Messages.NotificationCountMsg));

                _dataLoaded = true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetNotifications()", ex, NavigationService, Log);
            }

            SetProgressBar();
        }
    }
}
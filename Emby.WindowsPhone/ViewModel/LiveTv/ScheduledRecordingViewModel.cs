using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using Emby.WindowsPhone.Model.Interfaces;

namespace Emby.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScheduledRecordingViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ScheduledRecordingViewModel class.
        /// </summary>
        public ScheduledRecordingViewModel(INavigationService navigationService, IConnectionManager connectionManager) 
            : base(navigationService, connectionManager)
        {
        }

        public TimerInfoDto SelectedRecording { get; set; }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ScheduledRecordingChangedMsg))
                {
                    SelectedRecording = (TimerInfoDto) m.Sender;
                    //ServerIdItem = SelectedRecording;
                }
            });
        }
    }
}
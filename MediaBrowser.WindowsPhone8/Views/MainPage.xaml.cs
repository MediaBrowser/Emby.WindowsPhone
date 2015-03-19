using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class MainPage
    {
        private readonly PanoramaItem _inProgressPano;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _inProgressPano = InProgressPano;
            ShowHideInProgress(false);

            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShowHideInProgressMsg))
                {
                    var showInProgress = (bool) m.Sender;
                    ShowHideInProgress(showInProgress);
                }
            });
        }

        private void ShowHideInProgress(bool showInProgress)
        {
            if (showInProgress)
            {
                if (MainPano.Items.Count == 4)
                {
                    MainPano.Items.Add(_inProgressPano);
                }
            }
            else
            {
                if (MainPano.Items.Count == 5)
                {
                    MainPano.Items.RemoveAt(4);
                }
            }
        }
    }
}

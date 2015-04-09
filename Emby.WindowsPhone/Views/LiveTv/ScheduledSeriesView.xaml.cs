using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;

namespace Emby.WindowsPhone.Views.LiveTv
{
    public partial class ScheduledSeriesView
    {
        // Constructor
        public ScheduledSeriesView()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ScheduledSeriesCancelChangesMsg));
        }
    }
}
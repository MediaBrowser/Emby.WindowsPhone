using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.Model;
using Emby.WindowsPhone.Localisation;

namespace MediaBrowser.WindowsPhone.Views.LiveTv
{
    public partial class RecordedTvView
    {
        // Constructor
        public RecordedTvView()
        {
            InitializeComponent();
        }

        private void btnChangeGrouping_Click(object sender, System.EventArgs e)
        {

            new AppBarPrompt(
                new AppBarPromptAction(AppResources.AppBarRecordedDate.ToLower(), () => Messenger.Default.Send(new NotificationMessage(RecordedGroupBy.RecordedDate, Constants.Messages.ChangeRecordingGroupingMsg))),
                new AppBarPromptAction(AppResources.AppBarShowName.ToLower(), () => Messenger.Default.Send(new NotificationMessage(RecordedGroupBy.ShowName, Constants.Messages.ChangeRecordingGroupingMsg))),
                new AppBarPromptAction(AppResources.AppBarChannel.ToLower(), () => Messenger.Default.Send(new NotificationMessage(RecordedGroupBy.Channel, Constants.Messages.ChangeRecordingGroupingMsg)))).Show();
            
        }
    }
}
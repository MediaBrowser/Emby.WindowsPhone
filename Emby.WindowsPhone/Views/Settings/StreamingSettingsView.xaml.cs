using System;

namespace Emby.WindowsPhone.Views.Settings
{
    public partial class StreamingSettingsView
    {
        // Constructor
        public StreamingSettingsView()
        {
            InitializeComponent();
        }

        public void EmailLogs_OnClick(object sender, EventArgs e)
        {
            EmailLogs();
        }

        public void AboutItem_OnClick(object sender, EventArgs e)
        {
            AboutItem();
        }
    }
}
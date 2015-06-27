using System;

namespace Emby.WindowsPhone.Views.Settings
{
    public partial class LiveTvSettingsView
    {
        // Constructor
        public LiveTvSettingsView()
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
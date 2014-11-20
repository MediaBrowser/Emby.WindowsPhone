using System;
using Microsoft.Phone.Shell;

namespace MediaBrowser.WindowsPhone.Views.Settings
{
    public partial class MbConnectView 
    {
        // Constructor
        public MbConnectView()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                if (ApplicationBar != null && ApplicationBar.Mode == ApplicationBarMode.Default)
                {
                    UsernameBox.Focus();
                }
            };
        }

        private void EmailLogs_OnClick(object sender, EventArgs e)
        {
            EmailLogs();
        }

        private void AboutItem_OnClick(object sender, EventArgs e)
        {
            AboutItem();
        }
    }
}
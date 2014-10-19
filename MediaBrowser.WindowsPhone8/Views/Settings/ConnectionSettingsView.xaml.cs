using System;
using System.Windows.Input;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Views.Settings
{
    public partial class ConnectionSettingsView
    {
        // Constructor
        public ConnectionSettingsView()
        {
            InitializeComponent();
        }

        private void DisplayUrlButton_OnTap(object sender, GestureEventArgs e)
        {
            var content = DisplayUrlButton.Content as string;
            if (content != null)
            {
                new WebBrowserTask
                {
                    Uri = new Uri(content)
                }.Show();
            }
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
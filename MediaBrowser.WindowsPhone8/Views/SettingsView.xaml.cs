using System;
using Microsoft.Phone.Tasks;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class SettingsView
    {
        public SettingsView()
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
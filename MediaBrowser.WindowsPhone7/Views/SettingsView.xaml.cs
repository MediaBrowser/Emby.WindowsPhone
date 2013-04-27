using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class SettingsView : PhoneApplicationPage
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string settingPane;
            if (NavigationContext.QueryString.TryGetValue("settingsPane", out settingPane))
            {
                var selectedIndex = int.Parse(settingPane);
                settingsPivot.SelectedIndex = selectedIndex;
            }
        }

        private void EmailLogs_OnClick(object sender, EventArgs e)
        {
            new EmailComposeTask
            {
                To = "scottisafool@live.co.uk",
                Subject = string.Format("Media Browser 3 log file"),
                Body = WPLogger.GetLogFileContent()
            }.Show();
        }
    }
}
using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string settingPane;
            if (NavigationContext.QueryString.TryGetValue("settingsPane", out settingPane))
            {
                var selectedIndex = int.Parse(settingPane);
                if (selectedIndex >= settingsPivot.Items.Count)
                {
                    return;
                }

                settingsPivot.SelectedIndex = selectedIndex;
                var pivotsToRemove = settingsPivot.Items.Cast<PivotItem>().Where(x => x.Header.ToString() != "connection").ToList();
                foreach (var pivot in pivotsToRemove)
                {
                    settingsPivot.Items.Remove(pivot);
                }
            }
        }

        private void EmailLogs_OnClick(object sender, EventArgs e)
        {
            new EmailComposeTask
            {
                To = "wpmb3@outlook.com",
                Subject = string.Format("Media Browser 3 log file"),
                Body = WPLogger.GetLogs()
            }.Show();
        }
    }
}
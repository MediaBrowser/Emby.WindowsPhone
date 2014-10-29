using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Views.FirstRun
{
    public partial class ConfigureView
    {
        public ConfigureView()
        {
            InitializeComponent();
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.FirstRun.MbConnectFirstRunView, UriKind.Relative));
        }

        private void QuitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Terminate();
        }

        private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask
            {
                Uri = new Uri("http://mediabrowser3.com/download", UriKind.Absolute)
            }.Show();
        }
    }
}
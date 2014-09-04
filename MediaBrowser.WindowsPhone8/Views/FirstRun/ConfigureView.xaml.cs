using System;
using System.Windows;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Views.FirstRun
{
    public partial class ConfigureView
    {
        public ConfigureView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var appSettings = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();

            appSettings.Set(Constants.Settings.DoNotShowFirstRun, true);
            appSettings.Save();
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.SettingsViewConnection, UriKind.Relative));
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
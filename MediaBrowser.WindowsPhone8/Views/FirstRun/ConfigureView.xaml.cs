using System;
using System.Windows;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;

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
    }
}
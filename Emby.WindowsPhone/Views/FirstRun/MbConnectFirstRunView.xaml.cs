using System;
using System.Windows.Input;
using System.Windows.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.Views.FirstRun
{
    public partial class MbConnectFirstRunView
    {
        // Constructor
        public MbConnectFirstRunView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var appSettings = SimpleIoc.Default.GetInstance<IApplicationSettingsService>().Legacy;

            appSettings.Set(Constants.Settings.DoNotShowFirstRun, true);
        }

        private void LoginButton_OnTap(object sender, GestureEventArgs e)
        {
            Navigate(Constants.Pages.SettingsViews.MbConnectView);
        }

        private void SignUpButton_OnTap(object sender, GestureEventArgs e)
        {
            SimpleIoc.Default.GetInstance<INavigationService>().Navigate(Constants.Pages.SettingsViews.ConnectSignUpView);
        }

        private void SkipButton_OnTap(object sender, GestureEventArgs e)
        {
            Navigate(Constants.Pages.SettingsViewConnection);
        }

        private void Navigate(string uri)
        {
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
    }
}
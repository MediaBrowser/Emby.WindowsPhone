using System;
using System.Windows.Input;
using System.Windows.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;

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
            new LauncherService().LaunchUriAsync("http://emby.media/community/index.php?app=core&module=global&section=register");
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
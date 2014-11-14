using System;
using System.Windows.Input;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;

namespace MediaBrowser.WindowsPhone.Views.FirstRun
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

            var appSettings = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();

            appSettings.Set(Constants.Settings.DoNotShowFirstRun, true);
            appSettings.Save();
        }

        private void LoginButton_OnTap(object sender, GestureEventArgs e)
        {
            Navigate(Constants.Pages.SettingsViews.MbConnectView);
        }

        private void SignUpButton_OnTap(object sender, GestureEventArgs e)
        {
            new WebBrowserService().Show("http://mediabrowser.tv/community/index.php?app=core&module=global&section=register");
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
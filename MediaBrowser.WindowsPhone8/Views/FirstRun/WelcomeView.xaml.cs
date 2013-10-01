using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;

namespace MediaBrowser.WindowsPhone.Views.FirstRun
{
    public partial class WelcomeView
    {
        public WelcomeView()
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
    }
}
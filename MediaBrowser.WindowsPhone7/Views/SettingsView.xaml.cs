using System.Windows.Controls;
using Microsoft.Phone.Controls;

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
    }
}
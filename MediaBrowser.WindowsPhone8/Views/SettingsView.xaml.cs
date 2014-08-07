using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using ScottIsAFool.WindowsPhone.Logging;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
#if WP8
using Windows.System;
#endif

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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
                var pivotsToRemove = settingsPivot.Items.Cast<PivotItem>().Where(x => x.Header.ToString().ToLower() != AppResources.LabelConnection.ToLower()).ToList();
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

#if WP8
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                TileService.Current.SetSecondaryTileTransparency(App.SpecificSettings.UseTransparentTile);
                TileService.Current.UpdatePrimaryTile(App.SpecificSettings.DisplayBackdropOnTile, App.SpecificSettings.UseRichWideTile, App.SpecificSettings.UseTransparentTile).ConfigureAwait(false);
            }

            base.OnNavigatedFrom(e);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:", UriKind.Absolute));
        }
#endif

        private void AboutItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void DisplayUrlButton_OnTap(object sender, GestureEventArgs e)
        {
            var content = DisplayUrlButton.Content as string;
            if (content != null)
            {
                new WebBrowserTask
                {
                    Uri = new Uri(content)
                }.Show();
            }
        }
    }
}
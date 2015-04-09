using System;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ScottIsAFool.WindowsPhone.Controls;
using ScottIsAFool.WindowsPhone.Logging;

namespace Emby.WindowsPhone.Views
{
    public class SettingsBasePage : BasePage
    {
        public SettingsBasePage()
        {
            SystemTray.IsVisible = true;
            SystemTray.Opacity = 0;
        }

        public void EmailLogs()
        {
            new EmailComposeTask
            {
                To = "wpmb3@outlook.com",
                Subject = string.Format("Emby log file"),
                Body = WPLogger.GetLogs()
            }.Show();
        }

        public void AboutItem()
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }
    }
}
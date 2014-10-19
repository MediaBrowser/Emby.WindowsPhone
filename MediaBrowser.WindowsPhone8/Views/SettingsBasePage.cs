using System;
using Microsoft.Phone.Tasks;
using ScottIsAFool.WindowsPhone.Controls;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Views
{
    public class SettingsBasePage : BasePage
    {
        public void EmailLogs()
        {
            new EmailComposeTask
            {
                To = "wpmb3@outlook.com",
                Subject = string.Format("Media Browser 3 log file"),
                Body = WPLogger.GetLogs()
            }.Show();
        }

        public void AboutItem()
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }
    }
}
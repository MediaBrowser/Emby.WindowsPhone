using System.Windows;
using Microsoft.Phone.Tasks;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class LogFileView
    {
        public LogFileView()
        {
            InitializeComponent();
        }

        private void SendLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask
            {
                To = "scottisafool@live.co.uk",
                Subject = string.Format("Media Browser 3 log file"),
                Body = WPLogger.GetLogs()
            }.Show();
        }

        private void ClearLogsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear your logs?", "Are you sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                WPLogger.ClearLogs();
            }
        }
    }
}

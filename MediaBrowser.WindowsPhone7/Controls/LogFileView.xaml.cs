using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class LogFileView : UserControl
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
                Body = WPLogger.GetLogFileContent()
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

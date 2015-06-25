using System;
using Emby.WindowsPhone.Model.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Phone.Shell;

namespace Emby.WindowsPhone.Views.Settings
{
    public partial class MbConnectView 
    {
        // Constructor
        public MbConnectView()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                if (ApplicationBar != null && ApplicationBar.Mode == ApplicationBarMode.Default)
                {
                    UsernameBox.Focus();
                }
            };
        }

        private void EmailLogs_OnClick(object sender, EventArgs e)
        {
            EmailLogs();
        }

        private void AboutItem_OnClick(object sender, EventArgs e)
        {
            AboutItem();
        }

        private void SignUpButton_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SimpleIoc.Default.GetInstance<INavigationService>().Navigate(Constants.Pages.SettingsViews.ConnectSignUpView);
        }
    }
}
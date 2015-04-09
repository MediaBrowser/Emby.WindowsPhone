using System;
using System.Windows;

namespace Emby.WindowsPhone.Views.FirstRun
{
    public partial class WelcomeView
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.FirstRun.ConfigureView, UriKind.Relative));
        }
    }
}
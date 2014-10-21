using System;
using System.Windows;
using System.Windows.Controls;
using Windows.System;
using MediaBrowser.WindowsPhone.ViewModel.Settings;

namespace MediaBrowser.WindowsPhone.Views.Settings
{
    public partial class LockScreenSettingsView
    {
        // Constructor
        public LockScreenSettingsView()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:", UriKind.Absolute));
        }

        private void ListPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as SettingsViewModel;
            if (vm != null)
            {
                vm.CollectionChangedCommand.Execute(e);
            }
        }

        public void EmailLogs_OnClick(object sender, EventArgs e)
        {
            EmailLogs();
        }

        public void AboutItem_OnClick(object sender, EventArgs e)
        {
            AboutItem();
        }
    }
}
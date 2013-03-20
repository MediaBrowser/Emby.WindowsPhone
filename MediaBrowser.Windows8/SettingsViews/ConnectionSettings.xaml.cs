using MediaBrowser.Windows8.ViewModel;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MediaBrowser.Windows8.SettingsViews
{
    public sealed partial class ConnectionSettings : UserControl
    {
        public ConnectionSettings()
        {
            this.InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ((SettingsViewModel)DataContext).ServerTappedCommand.Execute(e.ClickedItem);
        }
    }
}

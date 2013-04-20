using MediaBrowser.Windows8.ViewModel;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MediaBrowser.Windows8.SettingsViews
{
    public sealed partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            this.InitializeComponent();
            var version = Package.Current.Id.Version;
            VersionText.Text = string.Format("{0}.{1}.{2}.{3}",
                                             version.Major,
                                             version.Minor,
                                             version.Build,
                                             version.Revision);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ((SettingsViewModel)ConnectionStack.DataContext).ServerTappedCommand.Execute(e.ClickedItem);
        }
    }
}

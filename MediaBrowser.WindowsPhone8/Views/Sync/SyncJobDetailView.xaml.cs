using System.Windows.Controls;
using MediaBrowser.WindowsPhone.ViewModel.Sync;

namespace MediaBrowser.WindowsPhone.Views.Sync
{
    public partial class SyncJobDetailView
    {
        // Constructor
        public SyncJobDetailView()
        {
            InitializeComponent();
        }

        private void LongListMultiSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as SyncJobDetailViewModel;
            if (vm != null)
            {
                vm.SelectionChangedCommand.Execute(e);
            }
        }
    }
}
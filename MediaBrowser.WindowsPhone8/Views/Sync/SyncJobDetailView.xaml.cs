using System.Windows.Controls;
using Emby.WindowsPhone.ViewModel.Sync;

namespace Emby.WindowsPhone.Views.Sync
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
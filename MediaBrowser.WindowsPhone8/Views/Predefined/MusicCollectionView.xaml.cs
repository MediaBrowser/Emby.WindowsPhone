using System.Windows.Controls;
using MediaBrowser.WindowsPhone.ViewModel.Predefined;

namespace MediaBrowser.WindowsPhone.Views.Predefined
{
    /// <summary>
    /// Description for MusicCollectionView.
    /// </summary>
    public partial class MusicCollectionView
    {
        /// <summary>
        /// Initializes a new instance of the MusicCollectionView class.
        /// </summary>
        public MusicCollectionView()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (SongSelector.IsSelectionEnabled)
            {
                SongSelector.IsSelectionEnabled = false;
                e.Cancel = true;
            }
        }

        private void SongSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as MusicCollectionViewModel;
            if (vm != null)
            {
                vm.SelectionChangedCommand.Execute(e);
            }
        }
    }
}
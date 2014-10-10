using System;
using System.Windows.Controls;
using MediaBrowser.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for AlbumView.
    /// </summary>
    public partial class AlbumView
    {
        /// <summary>
        /// Initializes a new instance of the AlbumView class.
        /// </summary>
        public AlbumView()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            MultiSelectList.IsSelectionEnabled = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (MultiSelectList.IsSelectionEnabled)
            {
                MultiSelectList.IsSelectionEnabled = false;
                e.Cancel = true;
            }
        }

        private void MultiSelectList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as MusicViewModel;
            if (vm != null)
            {
                vm.SelectionChangedCommand.Execute(e);
            }
        }
    }
}
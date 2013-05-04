using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for FullPlaylistView.
    /// </summary>
    public partial class FullPlaylistView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the FullPlaylistView class.
        /// </summary>
        public FullPlaylistView()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_OnClick(object sender, System.EventArgs e)
        {
            PlaylistSelector.IsSelectionEnabled = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (PlaylistSelector.IsSelectionEnabled)
            {
                PlaylistSelector.IsSelectionEnabled = false;
                e.Cancel = true;
            }
        }
    }
}
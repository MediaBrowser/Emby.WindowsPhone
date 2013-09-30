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
#if !WP8
            if (SongSelector.IsCheckModeActive)
            {
                SongSelector.IsCheckModeActive = false;
                e.Cancel = true;
            }
#else
            if (SongSelector.IsSelectionEnabled)
            {
                SongSelector.IsSelectionEnabled = false;
                e.Cancel = true;
            }
#endif
        }
    }
}
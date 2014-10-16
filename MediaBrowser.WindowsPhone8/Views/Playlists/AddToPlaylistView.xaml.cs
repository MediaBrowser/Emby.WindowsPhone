using System.Windows.Navigation;

namespace MediaBrowser.WindowsPhone.Views.Playlists
{
    public partial class AddToPlaylistView
    {
        // Constructor
        public AddToPlaylistView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}
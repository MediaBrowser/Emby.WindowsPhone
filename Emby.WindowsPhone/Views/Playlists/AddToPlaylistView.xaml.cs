using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Emby.WindowsPhone.Views.Playlists
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
            if (e.NavigationMode != NavigationMode.Back && !(e.Content is ListPickerPage))
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}
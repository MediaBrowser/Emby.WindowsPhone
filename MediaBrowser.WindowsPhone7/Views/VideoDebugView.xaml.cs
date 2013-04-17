using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for VideoDebugView.
    /// </summary>
    public partial class VideoDebugView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the VideoDebugView class.
        /// </summary>
        public VideoDebugView()
        {
            InitializeComponent();
        }

        private void BtnPlay_OnClick(object sender, RoutedEventArgs e)
        {
            new MediaPlayerLauncher
            {
                Orientation = MediaPlayerOrientation.Landscape,
                Media = new Uri(TxUrl.Text, UriKind.Absolute),
                Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop,
                //Location = MediaLocationType.Data
            }.Show();
        }
    }
}
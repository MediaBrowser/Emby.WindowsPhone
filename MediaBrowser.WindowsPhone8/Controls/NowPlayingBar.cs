using System.Windows;
using System.Windows.Controls;

namespace MediaBrowser.WindowsPhone.Controls
{
    public class NowPlayingBar : Control
    {
        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            "ImageUrl", typeof (string), typeof (NowPlayingBar), new PropertyMetadata(default(string)));

        public string ImageUrl
        {
            get { return (string) GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public static readonly DependencyProperty TrackNameProperty = DependencyProperty.Register(
            "TrackName", typeof (string), typeof (NowPlayingBar), new PropertyMetadata(default(string)));

        public string TrackName
        {
            get { return (string) GetValue(TrackNameProperty); }
            set { SetValue(TrackNameProperty, value); }
        }

        public static readonly DependencyProperty PlayPauseImageProperty = DependencyProperty.Register(
            "PlayPauseImage", typeof (string), typeof (NowPlayingBar), new PropertyMetadata(default(string)));

        public string PlayPauseImage
        {
            get { return (string) GetValue(PlayPauseImageProperty); }
            set { SetValue(PlayPauseImageProperty, value); }
        }

        public static readonly DependencyProperty ArtistProperty = DependencyProperty.Register(
            "Artist", typeof (string), typeof (NowPlayingBar), new PropertyMetadata(default(string)));

        public string Artist
        {
            get { return (string) GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        public NowPlayingBar()
        {
            DefaultStyleKey = typeof (NowPlayingBar);
        }
    }
}

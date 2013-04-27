using System.ComponentModel;

namespace MediaBrowser.Shared
{
    public class PlaylistItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string TrackName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public bool IsPlaying { get; set; }
        public string TrackUrl { get; set; }
        public string MediaBrowserId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

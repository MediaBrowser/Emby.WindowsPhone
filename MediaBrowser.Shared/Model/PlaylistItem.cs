using System.Diagnostics;
using PropertyChanged;

namespace Emby.WindowsPhone.Model
{
    [ImplementPropertyChanged]
    [DebuggerDisplay("Title: {TrackName}")]
    public class PlaylistItem 
    {
        public int Id { get; set; }
        public int OriginalId { get; set; }
        public bool IsJustAdded { get; set; }
        public string TrackName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public bool IsPlaying { get; set; }
        public string TrackUrl { get; set; }
        public string MediaBrowserId { get; set; }
        public string ImageUrl { get; set; }
        public string BackgroundImageUrl { get; set; }
        public long RunTimeTicks { get; set; }
    }
}

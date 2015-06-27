using System;
using System.Collections.Generic;
using Emby.WindowsPhone.Model;
using PropertyChanged;

namespace MediaBrowser.Model
{
    [ImplementPropertyChanged]
    public class Playlist
    {
        public Playlist()
        {
            PlaylistItems = new List<PlaylistItem>();
        }

        public List<PlaylistItem> PlaylistItems { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsShuffled { get; set; }
        public bool IsOnRepeat { get; set; }
    }
}
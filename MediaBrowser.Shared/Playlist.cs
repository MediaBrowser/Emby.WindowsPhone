using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MediaBrowser.Shared
{
    public class Playlist : INotifyPropertyChanged
    {
        public Playlist()
        {
            PlaylistItems = new List<PlaylistItem>();
        }

        public List<PlaylistItem> PlaylistItems { get; set; }
        public DateTime ModifiedDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
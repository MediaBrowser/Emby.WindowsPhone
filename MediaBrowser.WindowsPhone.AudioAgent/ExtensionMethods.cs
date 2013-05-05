using System;
using MediaBrowser.Shared;
using Microsoft.Phone.BackgroundAudio;

namespace MediaBrowser.WindowsPhone.AudioAgent
{
    public static class ExtensionMethods
    {
        public static AudioTrack ToAudioTrack(this PlaylistItem playlistItem)
        {
            var result = new AudioTrack();

            result.BeginEdit();

            result.Source = new Uri(playlistItem.TrackUrl, UriKind.Absolute);
            result.Title = playlistItem.TrackName;
            result.Artist = playlistItem.Artist;
            result.Album = playlistItem.Album;
            result.AlbumArt = new Uri(playlistItem.ImageUrl, UriKind.Absolute);

            result.EndEdit();

            return result;
        }
    }
}

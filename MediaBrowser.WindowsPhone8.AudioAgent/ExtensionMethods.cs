using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model;
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
            result.AlbumArt = !string.IsNullOrEmpty(playlistItem.ImageUrl) ? new Uri(playlistItem.ImageUrl, UriKind.Absolute) : null;
            result.Tag = playlistItem.MediaBrowserId;

            result.EndEdit();

            return result;
        }

        static readonly Random Random = new Random();

        public static List<T> Randomise<T>(this IEnumerable<T> sequence)
        {
            var retArray = sequence.ToArray();

            for (var i = 0; i < retArray.Length - 1; i += 1)
            {
                var swapIndex = Random.Next(i + 1, retArray.Length);
                var temp = retArray[i];
                retArray[i] = retArray[swapIndex];
                retArray[swapIndex] = temp;
            }

            return retArray.ToList();
        }
    }
}

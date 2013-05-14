using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.Shared;
using Newtonsoft.Json;

namespace MediaBrowser.WindowsPhone.AudioAgent
{
    public class PlaylistHelper
    {
        private readonly IStorageService _storageService;
        private readonly string _playlistFile;

        public PlaylistHelper(IStorageService storageService)
        {
            _storageService = storageService;
            _playlistFile = String.Format("{0}.json", Constants.CurrentPlaylist);
        }

        public void ClearPlaylist()
        {
            if(_storageService.FileExists(_playlistFile))
                _storageService.DeleteFile(_playlistFile);
        }

        public Playlist GetPlaylist()
        {
            if (!_storageService.FileExists(_playlistFile)) return new Playlist();

            using (var file = new StreamReader(_storageService.OpenFile(_playlistFile, FileMode.Open, FileAccess.Read)))
            {
                var json = file.ReadToEnd();

                try
                {
                    return JsonConvert.DeserializeObject<Playlist>(json);
                }
                catch
                {
                    return new Playlist();
                }
            }
        }

        public void SavePlaylist(Playlist list)
        {
            if (list == null) return;

            ClearPlaylist();

            list.ModifiedDate = DateTime.Now;
            
            using (var file = new StreamWriter(_storageService.OpenFile(_playlistFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                var json = JsonConvert.SerializeObject(list);

                file.Write(json);
            }
        }

        public void ResetTrackNumbers(Playlist playlist)
        {
            if (playlist == null) return;

            var i = 1;
            foreach (var item in playlist.PlaylistItems)
            {
                item.Id = i;
                i++;
            }

            SavePlaylist(playlist);
        }

        public bool RandomiseTrackNumbers(bool randomise)
        {
            var playlist = GetPlaylist();

            if (playlist == null 
                || !playlist.PlaylistItems.Any() 
                || playlist.IsShuffled == randomise) return false;

            playlist.IsShuffled = randomise;

            if (randomise)
            {
                var randomisedList = playlist.PlaylistItems.Randomise();

                playlist.PlaylistItems = randomisedList.OrderBy(x => x.Id).ToList();

                ResetTrackNumbers(playlist);
            }
            else
            {
                playlist.PlaylistItems.ForEach(item => item.Id = item.OriginalId);
                playlist.PlaylistItems = playlist.PlaylistItems.OrderBy(x => x.Id).ToList();

                SavePlaylist(playlist);
            }

            return true;
        }

        public void AddToPlaylist(List<PlaylistItem> list)
        {
            if (list == null || !list.Any()) return;

            var playlist = GetPlaylist();

            var items = playlist.PlaylistItems;

            list.ForEach(items.Add);

            ResetTrackNumbers(playlist);
        }

        public void RemoveFromPlaylist(List<PlaylistItem> list)
        {
            if (list == null || !list.Any()) return;

            var playlist = GetPlaylist();

            var items = playlist.PlaylistItems;

            var afterRemoval = items.Where(x => !list.Contains(x)).ToList();

            playlist.PlaylistItems = afterRemoval;

            ResetTrackNumbers(playlist);
        }

        
    }
}

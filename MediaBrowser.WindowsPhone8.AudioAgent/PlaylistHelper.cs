using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model;
using Newtonsoft.Json;

namespace MediaBrowser.WindowsPhone.AudioAgent
{
    public class PlaylistHelper
    {
        private readonly IStorageServiceHandler _storageService;
        private readonly string _playlistFile;

        public PlaylistHelper(IStorageService storageService)
        {
            _storageService = storageService.Local;
            _playlistFile = String.Format("{0}.json", Constants.Messages.CurrentPlaylist);
        }

        public async void ClearPlaylist()
        {
            if(await _storageService.FileExistsAsync(_playlistFile))
                await _storageService.DeleteFileAsync(_playlistFile);
        }

        public async Task<Playlist> GetPlaylist()
        {
            if (!await _storageService.FileExistsAsync(_playlistFile)) return new Playlist();

            using (var file = new StreamReader(await _storageService.OpenFileForReadAsync(_playlistFile)))
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

        public async void SavePlaylist(Playlist list)
        {
            if (list == null) return;

            ClearPlaylist();

            list.ModifiedDate = DateTime.Now;
            
            using (var file = new StreamWriter(await _storageService.OpenFileForReadAsync(_playlistFile)))
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
                if (item.IsJustAdded)
                {
                    item.IsJustAdded = false;
                    item.OriginalId = i;
                }
                item.Id = i;
                i++;
            }

            SavePlaylist(playlist);
        }

        public async Task<bool> RandomiseTrackNumbers(bool randomise)
        {
            var playlist = await GetPlaylist();

            if (playlist == null 
                || !playlist.PlaylistItems.Any() 
                || playlist.IsShuffled == randomise) return false;

            playlist.IsShuffled = randomise;

            if (randomise)
            {
                var randomisedList = playlist.PlaylistItems.Randomise();

                playlist.PlaylistItems = randomisedList;

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

        public async Task AddToPlaylist(List<PlaylistItem> list)
        {
            if (list == null || !list.Any()) return;

            var playlist = await GetPlaylist();

            var items = playlist.PlaylistItems;

            list.ForEach(items.Add);

            ResetTrackNumbers(playlist);
        }

        public async Task RemoveFromPlaylist(List<PlaylistItem> list)
        {
            if (list == null || !list.Any()) return;

            var playlist = await GetPlaylist();

            var items = playlist.PlaylistItems;

            var afterRemoval = items.Where(x => !list.Contains(x)).ToList();

            playlist.PlaylistItems = afterRemoval;

            ResetTrackNumbers(playlist);
        }

        public async Task SetRepeat(bool repeat)
        {
            var playlist = await GetPlaylist();

            if (playlist == null || playlist.IsOnRepeat == repeat) return;

            playlist.IsOnRepeat = repeat;

            SavePlaylist(playlist);
        }

        public async Task SetAllTracksToNotPlayingAndSave()
        {
            var list = await GetPlaylist();

            SetAllTracksToNotPlaying(list.PlaylistItems);

            SavePlaylist(list);
        }

        public void SetAllTracksToNotPlaying(List<PlaylistItem> list)
        {
            list.ForEach(item => item.IsPlaying = false);
        }
    }
}

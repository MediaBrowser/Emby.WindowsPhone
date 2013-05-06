using System.Collections.Generic;
using System.IO;
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
            _playlistFile = string.Format("{0}.json", Constants.CurrentPlaylist);
        }

        public void ClearPlaylist()
        {
            if(_storageService.FileExists(_playlistFile))
                _storageService.DeleteFile(_playlistFile);
        }

        public List<PlaylistItem> GetPlaylist()
        {
            if(!_storageService.FileExists(_playlistFile)) return new List<PlaylistItem>();

            using (var file = new StreamReader(_storageService.OpenFile(_playlistFile, FileMode.Open, FileAccess.Read)))
            {
                var json = file.ReadToEnd();

                return JsonConvert.DeserializeObject<List<PlaylistItem>>(json);
            }
        }

        public void SavePlaylist(List<PlaylistItem> list)
        {
            if (list == null) return;

            ClearPlaylist();
            
            using (var file = new StreamWriter(_storageService.OpenFile(_playlistFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                var json = JsonConvert.SerializeObject(list);

                file.Write(json);
            }
        }
    }
}

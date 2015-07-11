using System.Collections.Generic;
using System.Threading.Tasks;
using Emby.WindowsPhone.Converters;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Model;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Extensions
{
    public static class PlaylistItemExtensions
    {
        private static readonly ImageUrlConverter ImageConverter = new ImageUrlConverter();
        internal static async Task<PlaylistItem> ToPlaylistItem(this BaseItemDto item, IApiClient apiClient, IPlaybackManager playbackManager)
        {
            var profile = VideoProfileHelper.GetWindowsPhoneProfile();
            var options = new AudioOptions
            {
                Profile = profile,
                ItemId = item.Id,
                DeviceId = apiClient.DeviceId,
                MediaSources = item.MediaSources
            };

            var streamInfo = await playbackManager.GetAudioStreamInfo(App.ServerInfo.Id, options, true, apiClient);
            
            var streamUrl = streamInfo.ToUrl(apiClient.GetApiUrl("/"), apiClient.AccessToken);

            return new PlaylistItem
            {
                Album = item.Album,
                Artist = item.AlbumArtist,
                TrackName = item.Name,
                TrackUrl = streamUrl.Replace(App.ServerInfo.LocalAddress, !string.IsNullOrEmpty(App.ServerInfo.ManualAddress) ? App.ServerInfo.ManualAddress : App.ServerInfo.RemoteAddress),
                MediaBrowserId = item.Id,
                IsJustAdded = true,
                ImageUrl = (string)ImageConverter.Convert(item, typeof(string), null, null),
                BackgroundImageUrl = (string)ImageConverter.Convert(item, typeof(string), "backdrop", null),
                RunTimeTicks = item.RunTimeTicks ?? 0
            };
        }

        internal static async Task<List<PlaylistItem>> ToPlayListItems(this List<BaseItemDto> list, IApiClient apiClient, IPlaybackManager playbackManager)
        {
            var newList = new List<PlaylistItem>();
            list.ForEach(async item =>
            {
                var playlistItem = await item.ToPlaylistItem(apiClient, playbackManager);
                newList.Add(playlistItem);
            });

            return newList;
        }
    }
}

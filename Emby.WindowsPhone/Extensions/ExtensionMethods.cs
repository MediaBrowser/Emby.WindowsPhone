using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Search;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.Logging;

namespace Emby.WindowsPhone.Extensions
{
    internal static class ExtensionMethods
    {
        internal static LogLevel ToLogLevel(this LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    return LogLevel.Debug;
                case LogSeverity.Error:
                    return LogLevel.Error;
                case LogSeverity.Fatal:
                    return LogLevel.Fatal;
                case LogSeverity.Info:
                    return LogLevel.Info;
                case LogSeverity.Warn:
                    return LogLevel.Warning;
                default:
                    return LogLevel.Info;
            }
        }

        internal static async Task<PlaylistItem> ToPlaylistItem(this BaseItemDto item, IApiClient apiClient, IPlaybackManager playbackManager)
        {
            var profile = VideoProfileHelper.GetWindowsPhoneProfile();
            var options = new AudioOptions
            {
                Profile = profile,
                ItemId = item.Id,
                DeviceId = apiClient.DeviceId,
                MaxBitrate = 128,
                MediaSources = item.MediaSources
            };

            var streamInfo = await playbackManager.GetAudioStreamInfo(App.ServerInfo.Id, options, false, apiClient);

            var streamUrl = streamInfo.ToUrl(apiClient.GetApiUrl("/"), apiClient.AccessToken);

            var converter = new Converters.ImageUrlConverter();
            return new PlaylistItem
            {
                Album = item.Album,
                Artist = item.AlbumArtist,
                TrackName = item.Name,
                TrackUrl = streamUrl.Replace(App.ServerInfo.LocalAddress, !string.IsNullOrEmpty(App.ServerInfo.ManualAddress) ? App.ServerInfo.ManualAddress : App.ServerInfo.RemoteAddress),
                MediaBrowserId = item.Id,
                IsJustAdded = true,
                ImageUrl = (string) converter.Convert(item, typeof (string), null, null),
                BackgroundImageUrl = (string) converter.Convert(item, typeof (string), "backdrop", null),
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

        public static BaseItemDto ToBaseItemDto(this SearchHint searchHint)
        {
            var item = new BaseItemDto
            {
                Type = searchHint.Type,
                Name = searchHint.Name,
                Id = searchHint.ItemId,
                ImageTags = string.IsNullOrEmpty(searchHint.PrimaryImageTag) ? null : new Dictionary<ImageType, string> { { ImageType.Primary, searchHint.PrimaryImageTag} }
            };

            return item;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Search;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Extensions
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

        internal static PlaylistItem ToPlaylistItem(this BaseItemDto item, IApiClient apiClient)
        {
            var streamUrl = apiClient.GetAudioStreamUrl(new StreamOptions
            {
                AudioBitRate = 128,
                AudioCodec = "Mp3",
                ItemId = item.Id,
                OutputFileExtension = "mp3",
            });

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


        internal static async Task<List<PlaylistItem>> ToPlayListItems(this List<BaseItemDto> list, IApiClient apiClient)
        {
            var newList = new List<PlaylistItem>();
            await Task.Factory.StartNew(() => list.ForEach(item =>
            {
                var playlistItem = item.ToPlaylistItem(apiClient);
                newList.Add(playlistItem);
            }));

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

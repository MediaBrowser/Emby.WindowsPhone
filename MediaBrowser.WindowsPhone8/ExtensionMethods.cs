using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Search;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone
{
    public class Enum<T>
    {
        public static List<T> GetNames()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");
            }

            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            var items = fields.Select(field => (T) Enum.Parse(typeof (T), field.Name, true)).ToList();

            return items;
        }
    }

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

        internal static PlaylistItem ToPlaylistItem(this BaseItemDto item, IExtendedApiClient apiClient)
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
                TrackUrl = streamUrl,
                MediaBrowserId = item.Id,
                IsJustAdded = true,
                ImageUrl = (string) converter.Convert(item, typeof (string), null, null),
                BackgroundImageUrl = (string) converter.Convert(item, typeof (string), "backdrop", null)
            };
        }

        public static BaseItemDto ToBaseItemDto(this SearchHint searchHint)
        {
            var item = new BaseItemDto
            {
                Type = searchHint.Type,
                Name = searchHint.Name,
                Id = searchHint.ItemId
            };

            return item;
        }
    }
}

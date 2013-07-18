using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Search;
using MediaBrowser.Shared;
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

        internal static PlaylistItem ToPlaylistItem(this BaseItemDto item, ExtendedApiClient apiClient)
        {
            var streamUrl = apiClient.GetAudioStreamUrl(new StreamOptions
                                                      {
                                                          AudioBitRate = 128,
                                                          AudioCodec = AudioCodecs.Mp3,
                                                          ItemId = item.Id,
                                                          OutputFileExtension = "mp3",
                                                      });
            return new PlaylistItem
                       {
                           Album = item.Album,
                           Artist = item.Artists.Any() ? item.Artists[0] : "",
                           TrackName = item.Name,
                           TrackUrl = streamUrl,
                           MediaBrowserId = item.Id,
                           IsJustAdded = true,
                           ImageUrl = (string)new Converters.ImageUrlConverter().Convert(item, typeof(string), null, null),
                       };
        }

        public static void SetHorizontalOffset(this FrameworkElement fe, double offset)
        {
            var trans = new TranslateTransform()
            {
                X = offset
            };
            fe.RenderTransform = trans;

            fe.Tag = new Offset
            {
                Value = offset,
                Transform = trans
            };
        }

        public static Offset GetHorizontalOffset(this FrameworkElement fe)
        {
            return fe.Tag == null ? new Offset() : (Offset)fe.Tag;
        }

        public struct Offset
        {
            public double Value { get; set; }
            public TranslateTransform Transform { get; set; }
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

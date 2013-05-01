using System.Linq;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Logging;
using MediaBrowser.Shared;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone
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
                           ImageUrl = (string) new Converters.ImageUrlConverter().Convert(item, typeof(string), null, null)
                       };
        }
    }
}

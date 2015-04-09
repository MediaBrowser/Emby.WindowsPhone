using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Emby.WindowsPhone.Model;
using Emby.WindowsPhone.Services;

namespace Emby.WindowsPhone.Extensions
{
    public static class ApiClientExtensions
    {
        internal static async Task<List<PlaylistItem>> GetInstantMixPlaylist(this IApiClient apiClient, BaseItemDto item, IPlaybackManager playbackManager)
        {
            ItemsResult result;
            var query = new SimilarItemsQuery { UserId = AuthenticationService.Current.LoggedInUserId, Id = item.Id, Fields = new []{ ItemFields.MediaSources}};

            switch (item.Type)
            {
                case "Audio":
                    result = await apiClient.GetInstantMixFromSongAsync(query);
                    break;
                case "MusicArtist":
                    result = await apiClient.GetInstantMixFromArtistAsync(query);
                    break;
                case "MusicAlbum":
                    result = await apiClient.GetInstantMixFromAlbumAsync(query);
                    break;
                case "Genre":
                    result = await apiClient.GetInstantMixFromMusicGenreAsync(query);
                    break;
                default:
                    return new List<PlaylistItem>();
            }

            if (result == null || result.Items.IsNullOrEmpty())
            {
                return new List<PlaylistItem>();
            }

            return await result.Items.ToList().ToPlayListItems(apiClient, playbackManager);
        }
    }
}

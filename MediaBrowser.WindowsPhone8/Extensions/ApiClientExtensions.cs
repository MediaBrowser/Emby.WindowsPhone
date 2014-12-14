using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Services;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class ApiClientExtensions
    {
        internal static async Task<List<PlaylistItem>> GetInstantMixPlaylist(this IApiClient apiClient, BaseItemDto item)
        {
            ItemsResult result;
            var audioQuery = new SimilarItemsQuery { UserId = AuthenticationService.Current.LoggedInUserId, Id = item.Id, Fields = new []{ ItemFields.MediaSources}};
            var nameQuery = new SimilarItemsByNameQuery { Name = item.Name, UserId = AuthenticationService.Current.LoggedInUserId, Fields = new[] { ItemFields.MediaSources } };

            switch (item.Type)
            {
                case "Audio":
                    result = await apiClient.GetInstantMixFromSongAsync(audioQuery);
                    break;
                case "MusicArtist":
                    result = await apiClient.GetInstantMixFromArtistAsync(nameQuery);
                    break;
                case "MusicAlbum":
                    result = await apiClient.GetInstantMixFromAlbumAsync(audioQuery);
                    break;
                case "Genre":
                    result = await apiClient.GetInstantMixFromMusicGenreAsync(nameQuery);
                    break;
                default:
                    return new List<PlaylistItem>();
            }

            if (result == null || result.Items.IsNullOrEmpty())
            {
                return new List<PlaylistItem>();
            }

            return await result.Items.ToList().ToPlayListItems(apiClient);
        }
    }
}

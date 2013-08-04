using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.WebSocket;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Search;

namespace MediaBrowser.Design
{
    public class ExtendedApiClientDesign : IExtendedApiClient
    {
        public System.Threading.Tasks.Task RegisterDeviceAsync(string deviceId, string uri, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task DeleteDeviceAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateDeviceAsync(string deviceId, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task PushHeartbeatAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<DeviceSettings> GetDeviceSettingsAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Notifications.Notification> AddNotification(Model.Notifications.Notification notification)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Users.AuthenticationResult> AuthenticateUserAsync(string username, byte[] sha1Hash)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ClearUserItemRatingAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public string ClientName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentUserId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string DeviceId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string DeviceName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Threading.Tasks.Task<Model.Querying.AllThemeMediaResult> GetAllThemeMediaAsync(string userId, string itemId, bool inheritFromParents)
        {
            throw new NotImplementedException();
        }

        public Task<SearchHintResult> GetSearchHints(string userId, string searchTerm, int? startIndex = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public string GetArtImageUrl(Model.Dto.BaseItemDto item, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetMusicGenreAsync(string name)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetArtistAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public string GetArtistImageUrl(string name, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetArtistsAsync(Model.Querying.ArtistsQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetAudioStreamUrl(Model.Dto.StreamOptions options)
        {
            throw new NotImplementedException();
        }

        public string[] GetBackdropImageUrls(Model.Dto.BaseItemDto item, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Session.SessionInfoDto[]> GetClientSessionsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Globalization.CountryInfo[]> GetCountriesAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemReviewsResult> GetCriticReviews(string itemId, int? startIndex = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Globalization.CultureDto[]> GetCulturesAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Entities.DisplayPreferences> GetDisplayPreferencesAsync(string id, string userId, string client)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetGameGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public string GetGameGenreImageUrl(string name, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetGameGenresAsync(Model.Querying.ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public string GetGenreImageUrl(string name, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetGenresAsync(Model.Querying.ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetHlsAudioStreamUrl(Model.Dto.StreamOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetHlsVideoStreamUrl(Model.Dto.VideoStreamOptions options)
        {
            throw new NotImplementedException();
        }

        public ApiWebSocket WebSocketConnection { get; set; }

        public System.Threading.Tasks.Task<System.IO.Stream> GetImageStreamAsync(string url)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(string itemId, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(Model.Dto.BaseItemDto item, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Plugins.PluginInfo[]> GetInstalledPluginsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<string[]> GetIntrosAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetItemAsync(string id, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.ItemCounts> GetItemCountsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetItemsAsync(Model.Querying.ItemQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto[]> GetLocalTrailersAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        public string GetLogoImageUrl(Model.Dto.BaseItemDto item, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetMusicGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public string GetMusicGenreImageUrl(string name, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetMusicGenresAsync(Model.Querying.ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetNextUpAsync(Model.Querying.NextUpQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Notifications.NotificationResult> GetNotificationsAsync(Model.Notifications.NotificationQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Notifications.NotificationsSummary> GetNotificationsSummary(string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<List<Model.Entities.ParentalRating>> GetParentalRatingsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetPeopleAsync(Model.Querying.PersonsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetPersonAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public string GetPersonImageUrl(string name, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetPersonImageUrl(Model.Dto.BaseItemPerson item, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.UserDto[]> GetPublicUsersAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetRootFolderAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Tasks.TaskInfo> GetScheduledTaskAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Tasks.TaskInfo[]> GetScheduledTasksAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<System.IO.Stream> GetSerializedStreamAsync(string url)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Configuration.ServerConfiguration> GetServerConfigurationAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetSimilarAlbumsAsync(Model.Querying.SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetSimilarGamesAsync(Model.Querying.SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetSimilarMoviesAsync(Model.Querying.SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetSimilarSeriesAsync(Model.Querying.SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetSimilarTrailersAsync(Model.Querying.SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto[]> GetSpecialFeaturesAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.BaseItemDto> GetStudioAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public string GetStudioImageUrl(string name, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ItemsResult> GetStudiosAsync(Model.Querying.ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.System.SystemInfo> GetSystemInfoAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ThemeMediaResult> GetThemeSongsAsync(string userId, string itemId, bool inheritFromParents)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Querying.ThemeMediaResult> GetThemeVideosAsync(string userId, string itemId, bool inheritFromParents)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.UserDto> GetUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public string GetUserImageUrl(string userId, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetUserImageUrl(Model.Dto.UserDto user, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Model.Dto.UserDto[]> GetUsersAsync(Model.Querying.UserQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetVideoStreamUrl(Model.Dto.VideoStreamOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetYearImageUrl(int year, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetYearImageUrl(Model.Dto.BaseItemDto item, Model.Dto.ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<Model.ApiClient.HttpResponseEventArgs> HttpResponseReceived;

        public Model.Serialization.IJsonSerializer JsonSerializer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Threading.Tasks.Task MarkNotificationsRead(string userId, IEnumerable<Guid> notificationIdList, bool isRead)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<T> PostAsync<T>(string url, Dictionary<string, string> args) where T : class
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportPlaybackProgressAsync(string itemId, string userId, long? positionTicks, bool isPaused)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportPlaybackStartAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportPlaybackStoppedAsync(string itemId, string userId, long? positionTicks)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task RestartServerAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendBrowseCommandAsync(string sessionId, string itemId, string itemName, string itemType, string context)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendPlayCommandAsync(string sessionId, Model.Session.PlayRequest request)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendPlaystateCommandAsync(string sessionId, Model.Session.PlaystateRequest request)
        {
            throw new NotImplementedException();
        }

        public int ServerApiPort
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string ServerHostName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Threading.Tasks.Task UpdateDisplayPreferencesAsync(Model.Entities.DisplayPreferences displayPreferences, string userId, string client)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateFavoriteStatusAsync(string itemId, string userId, bool isFavorite)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateNotification(Model.Notifications.Notification notification)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdatePlayedStatusAsync(string itemId, string userId, bool wasPlayed)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateScheduledTaskTriggersAsync(Guid id, Model.Tasks.TaskTriggerInfo[] triggers)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateServerConfigurationAsync(Model.Configuration.ServerConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateUserItemRatingAsync(string itemId, string userId, bool likes)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string ApplicationVersion
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int? ImageQuality
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

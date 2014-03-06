using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Notifications;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Search;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Tasks;

namespace MediaBrowser.Design
{
    public class ExtendedApiClientDesign : IExtendedApiClient
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string GetApiUrl(string handler)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSearchHintsAsync(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task MarkNotificationsRead(string userId, IEnumerable<Guid> notificationIdList, bool isRead)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task AddNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetNotificationsSummary(string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetNotificationsAsync(NotificationQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetItemAsync(string id, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetIntrosAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetRootFolderAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetUsersAsync(UserQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetClientSessionsAsync(SessionQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetItemCountsAsync(ItemCountsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetEpisodesAsync(EpisodeQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSeasonsAsync(SeasonQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetItemsAsync(ItemQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetInstantMixFromSongAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetInstantMixFromAlbumAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetInstantMixFromArtistAsync(SimilarItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetInstantMixFromMusicGenreAsync(SimilarItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSimilarMoviesAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSimilarTrailersAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSimilarSeriesAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSimilarAlbumsAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSimilarGamesAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetPeopleAsync(PersonsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetArtistsAsync(ArtistsQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetStudioAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetNextUpAsync(NextUpQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetMusicGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetGameGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetStudiosAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetMusicGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetGameGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetArtistAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task RestartServerAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSystemInfoAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetPersonAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetInstalledPluginsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetServerConfigurationAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetScheduledTasksAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetScheduledTaskAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetParentalRatingsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLocalTrailersAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSpecialFeaturesAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetCulturesAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetCountriesAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task MarkPlayedAsync(string itemId, string userId, DateTime? datePlayed)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task MarkUnplayedAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateFavoriteStatusAsync(string itemId, string userId, bool isFavorite)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportPlaybackStartAsync(string itemId, string userId, bool isSeekable, List<string> queueableMediaTypes)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportPlaybackProgressAsync(string itemId, string userId, long? positionTicks, bool isPaused, bool isMuted)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportPlaybackStoppedAsync(string itemId, string userId, long? positionTicks)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendBrowseCommandAsync(string sessionId, string itemId, string itemName, string itemType, string context)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendPlaystateCommandAsync(string sessionId, PlaystateRequest request)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendPlayCommandAsync(string sessionId, PlayRequest request)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendSystemCommandAsync(string sessionId, SystemCommand command)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task SendMessageCommandAsync(string sessionId, MessageCommand command)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ClearUserItemRatingAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateUserItemRatingAsync(string itemId, string userId, bool likes)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task AuthenticateUserAsync(string username, byte[] sha1Hash)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateServerConfigurationAsync(ServerConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateScheduledTaskTriggersAsync(Guid id, TaskTriggerInfo[] triggers)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetSerializedStreamAsync(string url)
        {
            throw new NotImplementedException();
        }

        public void ChangeServerLocation(string hostName, int apiPort)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(BaseItemDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(ChannelInfoDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(RecordingInfoDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(ProgramInfoDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(string itemId, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetUserImageUrl(UserDto user, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetUserImageUrl(string userId, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetPersonImageUrl(BaseItemPerson item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetPersonImageUrl(string name, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetYearImageUrl(BaseItemDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetYearImageUrl(int year, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetGenreImageUrl(string name, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetMusicGenreImageUrl(string name, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetGameGenreImageUrl(string name, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetStudioImageUrl(string name, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetArtistImageUrl(string name, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string[] GetBackdropImageUrls(BaseItemDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetLogoImageUrl(BaseItemDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetArtImageUrl(BaseItemDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetThumbImageUrl(BaseItemDto item, ImageOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetAudioStreamUrl(StreamOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetVideoStreamUrl(VideoStreamOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetHlsAudioStreamUrl(StreamOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetHlsVideoStreamUrl(VideoStreamOptions options)
        {
            throw new NotImplementedException();
        }

        public IJsonSerializer JsonSerializer { get; set; }
        public string ServerHostName { get; private set; }
        public int ServerApiPort { get; private set; }
        public string ClientName { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string CurrentUserId { get; set; }
        public event EventHandler ServerLocationChanged;
        public event EventHandler<HttpResponseEventArgs> HttpResponseReceived;
        public System.Threading.Tasks.Task DeleteLiveTvRecordingAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task CancelLiveTvSeriesTimerAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task CancelLiveTvTimerAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvSeriesTimerAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvSeriesTimersAsync(SeriesTimerQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvTimerAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetRecommendedLiveTvProgramsAsync(RecommendedProgramQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvProgramsAsync(ProgramQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvTimersAsync(TimerQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvRecordingGroupAsync(string id, string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvRecordingGroupsAsync(RecordingGroupQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvRecordingAsync(string id, string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvRecordingsAsync(RecordingQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvChannelAsync(string id, string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvChannelsAsync(ChannelQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetLiveTvInfoAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task PostAsync<T>(string url, Dictionary<string, string> args, System.Threading.CancellationToken cancellationToken) where T : class
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateDisplayPreferencesAsync(DisplayPreferences displayPreferences, string userId, string client, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetDisplayPreferencesAsync(string id, string userId, string client, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetPublicUsersAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetImageStreamAsync(string url, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetAllThemeMediaAsync(string userId, string itemId, bool inheritFromParents, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetThemeVideosAsync(string userId, string itemId, bool inheritFromParents, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetThemeSongsAsync(string userId, string itemId, bool inheritFromParents, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetCriticReviews(string itemId, System.Threading.CancellationToken cancellationToken, int? startIndex = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetYearIndex(string userId, string[] includeItemTypes, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetGamePlayerIndex(string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ReportCapabilities(string sessionId, ClientCapabilities capabilities, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetAsync<T>(string url, System.Threading.CancellationToken cancellationToken) where T : class
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task GetGameSystemSummariesAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RegisterDeviceAsync(string deviceId, string uri, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeviceAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(string deviceId, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            throw new NotImplementedException();
        }

        public Task PushHeartbeatAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceSettings> GetDeviceSettingsAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        Task<Notification> IApiClient.AddNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        Task<Model.Users.AuthenticationResult> IApiClient.AuthenticateUserAsync(string username, byte[] sha1Hash)
        {
            throw new NotImplementedException();
        }

        Task<UserItemDataDto> IApiClient.ClearUserItemRatingAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        Task<AllThemeMediaResult> IApiClient.GetAllThemeMediaAsync(string userId, string itemId, bool inheritFromParents, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetArtistAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetArtistsAsync(ArtistsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<T> IApiClient.GetAsync<T>(string url, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<SessionInfoDto[]> IApiClient.GetClientSessionsAsync(SessionQuery query)
        {
            throw new NotImplementedException();
        }

        Task<Model.Globalization.CountryInfo[]> IApiClient.GetCountriesAsync()
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<ItemReview>> IApiClient.GetCriticReviews(string itemId, System.Threading.CancellationToken cancellationToken, int? startIndex = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        Task<Model.Globalization.CultureDto[]> IApiClient.GetCulturesAsync()
        {
            throw new NotImplementedException();
        }

        Task<DisplayPreferences> IApiClient.GetDisplayPreferencesAsync(string id, string userId, string client, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetEpisodesAsync(EpisodeQuery query)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetGameGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetGameGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        Task<List<ItemIndex>> IApiClient.GetGamePlayerIndex(string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<List<GameSystemSummary>> IApiClient.GetGameSystemSummariesAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        Task<System.IO.Stream> IApiClient.GetImageStreamAsync(string url, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<Model.Plugins.PluginInfo[]> IApiClient.GetInstalledPluginsAsync()
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetInstantMixFromAlbumAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetInstantMixFromArtistAsync(SimilarItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetInstantMixFromMusicGenreAsync(SimilarItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetInstantMixFromSongAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetIntrosAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetItemAsync(string id, string userId)
        {
            throw new NotImplementedException();
        }

        Task<ItemCounts> IApiClient.GetItemCountsAsync(ItemCountsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetItemsAsync(ItemQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ChannelInfoDto> IApiClient.GetLiveTvChannelAsync(string id, string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<ChannelInfoDto>> IApiClient.GetLiveTvChannelsAsync(ChannelQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<LiveTvInfo> IApiClient.GetLiveTvInfoAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<ProgramInfoDto>> IApiClient.GetLiveTvProgramsAsync(ProgramQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<RecordingInfoDto> IApiClient.GetLiveTvRecordingAsync(string id, string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<RecordingGroupDto> IApiClient.GetLiveTvRecordingGroupAsync(string id, string userId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<RecordingGroupDto>> IApiClient.GetLiveTvRecordingGroupsAsync(RecordingGroupQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<RecordingInfoDto>> IApiClient.GetLiveTvRecordingsAsync(RecordingQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<SeriesTimerInfoDto> IApiClient.GetLiveTvSeriesTimerAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<SeriesTimerInfoDto>> IApiClient.GetLiveTvSeriesTimersAsync(SeriesTimerQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<TimerInfoDto> IApiClient.GetLiveTvTimerAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<TimerInfoDto>> IApiClient.GetLiveTvTimersAsync(TimerQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto[]> IApiClient.GetLocalTrailersAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetMusicGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetMusicGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        Task<NotificationResult> IApiClient.GetNotificationsAsync(NotificationQuery query)
        {
            throw new NotImplementedException();
        }

        Task<NotificationsSummary> IApiClient.GetNotificationsSummary(string userId)
        {
            throw new NotImplementedException();
        }

        Task<List<ParentalRating>> IApiClient.GetParentalRatingsAsync()
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetPeopleAsync(PersonsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetPersonAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        Task<UserDto[]> IApiClient.GetPublicUsersAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<QueryResult<ProgramInfoDto>> IApiClient.GetRecommendedLiveTvProgramsAsync(RecommendedProgramQuery query, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetRootFolderAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task<TaskInfo> IApiClient.GetScheduledTaskAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<TaskInfo[]> IApiClient.GetScheduledTasksAsync()
        {
            throw new NotImplementedException();
        }

        Task<SearchHintResult> IApiClient.GetSearchHintsAsync(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetSeasonsAsync(SeasonQuery query)
        {
            throw new NotImplementedException();
        }

        Task<System.IO.Stream> IApiClient.GetSerializedStreamAsync(string url)
        {
            throw new NotImplementedException();
        }

        Task<ServerConfiguration> IApiClient.GetServerConfigurationAsync()
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetSimilarAlbumsAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetSimilarGamesAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetSimilarMoviesAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetSimilarSeriesAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetSimilarTrailersAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto[]> IApiClient.GetSpecialFeaturesAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        Task<BaseItemDto> IApiClient.GetStudioAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        Task<ItemsResult> IApiClient.GetStudiosAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        Task<Model.System.SystemInfo> IApiClient.GetSystemInfoAsync()
        {
            throw new NotImplementedException();
        }

        Task<ThemeMediaResult> IApiClient.GetThemeSongsAsync(string userId, string itemId, bool inheritFromParents, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ThemeMediaResult> IApiClient.GetThemeVideosAsync(string userId, string itemId, bool inheritFromParents, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<UserDto> IApiClient.GetUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        Task<UserDto[]> IApiClient.GetUsersAsync(UserQuery query)
        {
            throw new NotImplementedException();
        }

        Task<List<ItemIndex>> IApiClient.GetYearIndex(string userId, string[] includeItemTypes, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<UserItemDataDto> IApiClient.MarkPlayedAsync(string itemId, string userId, DateTime? datePlayed)
        {
            throw new NotImplementedException();
        }

        Task<UserItemDataDto> IApiClient.MarkUnplayedAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        Task<T> IApiClient.PostAsync<T>(string url, Dictionary<string, string> args, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<UserItemDataDto> IApiClient.UpdateFavoriteStatusAsync(string itemId, string userId, bool isFavorite)
        {
            throw new NotImplementedException();
        }

        Task<UserItemDataDto> IApiClient.UpdateUserItemRatingAsync(string itemId, string userId, bool likes)
        {
            throw new NotImplementedException();
        }


        public Task<ItemsResult> GetNextUpEpisodesAsync(NextUpQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetUpcomingEpisodesAsync(NextUpQuery query)
        {
            throw new NotImplementedException();
        }
    }
}

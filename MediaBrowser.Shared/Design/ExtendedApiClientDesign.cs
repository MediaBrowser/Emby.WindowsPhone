using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Notifications;
using MediaBrowser.Model.Playlists;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Search;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Tasks;
using MediaBrowser.Model.Users;

namespace MediaBrowser.WindowsPhone.Design
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

        public Task<List<GameSystemSummary>> GetGameSystemSummariesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = new CancellationToken()) where T : class
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

        public string GetHlsVideoStreamUrl(VideoStreamOptions options)
        {
            throw new NotImplementedException();
        }

        public Task ReportCapabilities(ClientCapabilities capabilities, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task<List<ItemIndex>> GetGamePlayerIndex(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<List<ItemIndex>> GetYearIndex(string userId, string[] includeItemTypes, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<ItemReview>> GetCriticReviews(string itemId, CancellationToken cancellationToken = new CancellationToken(), int? startIndex = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public Task<ThemeMediaResult> GetThemeSongsAsync(string userId, string itemId, bool inheritFromParents, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<SearchHintResult> GetSearchHintsAsync(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ThemeMediaResult> GetThemeVideosAsync(string userId, string itemId, bool inheritFromParents, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<AllThemeMediaResult> GetAllThemeMediaAsync(string userId, string itemId, bool inheritFromParents, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task MarkNotificationsRead(string userId, IEnumerable<string> notificationIdList, bool isRead)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationsSummary> GetNotificationsSummary(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationResult> GetNotificationsAsync(NotificationQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetImageStreamAsync(string url, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetItemAsync(string id, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<BaseItemDto>> GetLatestItems(LatestItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetIntrosAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetRootFolderAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetAdditionalParts(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto[]> GetUsersAsync(UserQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto[]> GetPublicUsersAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<SessionInfoDto[]> GetClientSessionsAsync(SessionQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<SessionInfoDto> GetCurrentSessionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemCounts> GetItemCountsAsync(ItemCountsQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetEpisodesAsync(EpisodeQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetSeasonsAsync(SeasonQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetItemsAsync(ItemQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetUserViews(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetInstantMixFromSongAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetInstantMixFromAlbumAsync(SimilarItemsQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetInstantMixFromArtistAsync(SimilarItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetInstantMixFromMusicGenreAsync(SimilarItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetSimilarMoviesAsync(SimilarItemsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetSimilarTrailersAsync(SimilarItemsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetSimilarSeriesAsync(SimilarItemsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetSimilarAlbumsAsync(SimilarItemsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetSimilarGamesAsync(SimilarItemsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetPeopleAsync(PersonsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetArtistsAsync(ArtistsQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetAlbumArtistsAsync(ArtistsQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetStudioAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetNextUpEpisodesAsync(NextUpQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetUpcomingEpisodesAsync(NextUpQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetMusicGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetGameGenresAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ItemsResult> GetStudiosAsync(ItemsByNameQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetMusicGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetGameGenreAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetArtistAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public Task RestartServerAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<PublicSystemInfo> GetPublicSystemInfoAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto> GetPersonAsync(string name, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<PluginInfo[]> GetInstalledPluginsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServerConfiguration> GetServerConfigurationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TaskInfo[]> GetScheduledTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TaskInfo> GetScheduledTaskAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ParentalRating>> GetParentalRatingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto[]> GetLocalTrailersAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseItemDto[]> GetSpecialFeaturesAsync(string userId, string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<CultureDto[]> GetCulturesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CountryInfo[]> GetCountriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserItemDataDto> MarkPlayedAsync(string itemId, string userId, DateTime? datePlayed)
        {
            throw new NotImplementedException();
        }

        public Task<UserItemDataDto> MarkUnplayedAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserItemDataDto> UpdateFavoriteStatusAsync(string itemId, string userId, bool isFavorite)
        {
            throw new NotImplementedException();
        }

        public Task ReportPlaybackStartAsync(PlaybackStartInfo info)
        {
            throw new NotImplementedException();
        }

        public Task ReportPlaybackProgressAsync(PlaybackProgressInfo info)
        {
            throw new NotImplementedException();
        }

        public Task ReportPlaybackStoppedAsync(PlaybackStopInfo info)
        {
            throw new NotImplementedException();
        }

        public Task SendBrowseCommandAsync(string sessionId, string itemId, string itemName, string itemType)
        {
            throw new NotImplementedException();
        }

        public Task SendPlaystateCommandAsync(string sessionId, PlaystateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task SendPlayCommandAsync(string sessionId, PlayRequest request)
        {
            throw new NotImplementedException();
        }

        public Task SendCommandAsync(string sessionId, GeneralCommand command)
        {
            throw new NotImplementedException();
        }

        public Task SendString(string sessionId, string text)
        {
            throw new NotImplementedException();
        }

        public Task SetVolume(string sessionId, int volume)
        {
            throw new NotImplementedException();
        }

        public Task StopTranscodingProcesses(string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task SetAudioStreamIndex(string sessionId, int index)
        {
            throw new NotImplementedException();
        }

        public Task SetSubtitleStreamIndex(string sessionId, int? index)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageCommandAsync(string sessionId, MessageCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<UserItemDataDto> ClearUserItemRatingAsync(string itemId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserItemDataDto> UpdateUserItemRatingAsync(string itemId, string userId, bool likes)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> AuthenticateUserAsync(string username, byte[] sha1Hash)
        {
            throw new NotImplementedException();
        }

        public Task UpdateServerConfigurationAsync(ServerConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public Task UpdateScheduledTaskTriggersAsync(string id, TaskTriggerInfo[] triggers)
        {
            throw new NotImplementedException();
        }

        public Task<DisplayPreferences> GetDisplayPreferencesAsync(string id, string userId, string client, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task UpdateDisplayPreferencesAsync(DisplayPreferences displayPreferences, string userId, string client, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<T> PostAsync<T>(string url, Dictionary<string, string> args, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetSerializedStreamAsync(string url)
        {
            throw new NotImplementedException();
        }

        public void ChangeServerLocation(string address)
        {
            throw new NotImplementedException();
        }

        public void SetAuthenticationInfo(string accessToken, string userId)
        {
            throw new NotImplementedException();
        }

        public void SetAuthenticationInfo(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void ClearAuthenticationInfo()
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

        public string GetSubtitleUrl(SubtitleDownloadOptions options)
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

        public Task<LiveTvInfo> GetLiveTvInfoAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<ChannelInfoDto>> GetLiveTvChannelsAsync(LiveTvChannelQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ChannelInfoDto> GetLiveTvChannelAsync(string id, string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<RecordingInfoDto>> GetLiveTvRecordingsAsync(RecordingQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<RecordingInfoDto> GetLiveTvRecordingAsync(string id, string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<RecordingGroupDto>> GetLiveTvRecordingGroupsAsync(RecordingGroupQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<RecordingGroupDto> GetLiveTvRecordingGroupAsync(string id, string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<TimerInfoDto>> GetLiveTvTimersAsync(TimerQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<ProgramInfoDto>> GetLiveTvProgramsAsync(ProgramQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ProgramInfoDto> GetLiveTvProgramAsync(string id, string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<ProgramInfoDto>> GetRecommendedLiveTvProgramsAsync(RecommendedProgramQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CreateLiveTvTimerAsync(BaseTimerInfoDto timer, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task UpdateLiveTvTimerAsync(TimerInfoDto timer, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CreateLiveTvSeriesTimerAsync(SeriesTimerInfoDto timer, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task UpdateLiveTvSeriesTimerAsync(SeriesTimerInfoDto timer, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<TimerInfoDto> GetLiveTvTimerAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<SeriesTimerInfoDto>> GetLiveTvSeriesTimersAsync(SeriesTimerQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<SeriesTimerInfoDto> GetLiveTvSeriesTimerAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CancelLiveTvTimerAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CancelLiveTvSeriesTimerAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteLiveTvRecordingAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<SeriesTimerInfoDto> GetDefaultLiveTvTimerInfo(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<GuideInfo> GetLiveTvGuideInfo(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<SeriesTimerInfoDto> GetDefaultLiveTvTimerInfo(string programId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<ChannelFeatures> GetChannelFeatures(string channelId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<BaseItemDto>> GetChannelItems(ChannelItemQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<BaseItemDto>> GetChannels(ChannelQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<BaseItemDto>> GetLatestChannelItems(AllChannelMediaQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
        
        public Task AddToPlaylist(string playlistId, IEnumerable<string> itemIds, string userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromPlaylist(string playlistId, IEnumerable<string> entryIds)
        {
            throw new NotImplementedException();
        }

        public IJsonSerializer JsonSerializer { get; set; }
        public string ServerAddress { get; private set; }
        public string ClientName { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string CurrentUserId { get; private set; }
        public string AccessToken { get; private set; }
        public event EventHandler ServerLocationChanged;
        public event EventHandler<HttpResponseEventArgs> HttpResponseReceived;
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
    }
}

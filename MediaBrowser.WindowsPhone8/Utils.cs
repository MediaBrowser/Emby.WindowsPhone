using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction;
using MediaBrowser.ApiInteraction.WebSocket;
using MediaBrowser.Model;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using Newtonsoft.Json;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.Logging;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone
{
    public static class Utils
    {
        public static List<Group<BaseItemPerson>> GroupCastAndCrew(IEnumerable<BaseItemPerson> people)
        {
            var emptyGroups = new List<Group<BaseItemPerson>>();
            var headers = new List<string> { AppResources.LabelDirector, AppResources.LabelActor, AppResources.LabelWriter, AppResources.LabelProducer, AppResources.LabelOther };
            headers.ForEach(item => emptyGroups.Add(new Group<BaseItemPerson>(item, new List<BaseItemPerson>())));

            var groupedPeople = (from p in people
                                 group p by GetPersonType(p.Type)
                                     into grp
                                     orderby grp.Key
                                     select new Group<BaseItemPerson>(grp.Key, grp)).ToList();

            var result = (from g in groupedPeople.Union(emptyGroups)
#if WP8
                          where g.Count > 0
#else
                          where g.HasItems
#endif
                          orderby g.Title
                          select g).ToList();

            return result;
        }

        public static async Task<List<Group<BaseItemDto>>> GroupItemsByName(IEnumerable<BaseItemDto> items)
        {
            return await TaskEx.Run(() =>
            {
                var emptyGroups = new List<Group<BaseItemDto>>();

                var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                headers.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item, new List<BaseItemDto>())));

                var groupedTracks = (from t in items
                                     group t by GetSortByNameHeader(t)
                                         into grp
                                         orderby grp.Key
                                         select new Group<BaseItemDto>(grp.Key, grp)).ToList();

                var result = (from g in groupedTracks.Union(emptyGroups)
#if WP8
                              where g.Count > 0
#else
                          where g.HasItems



#endif
                              orderby g.Title
                              select g).ToList();

                return result;
            });
        }

        internal static string GetSortByNameHeader(BaseItemDto dtoBaseItem)
        {
            if (String.IsNullOrEmpty(dtoBaseItem.Name) && String.IsNullOrEmpty(dtoBaseItem.SortName))
            {
                return '#'.ToString(CultureInfo.InvariantCulture);
            }

            var name = !String.IsNullOrEmpty(dtoBaseItem.SortName) ? dtoBaseItem.SortName : dtoBaseItem.Name;
            return SortByNameHeader(name);
        }

        internal static string GetSortByNameHeader(ChannelInfoDto channelInfoDto)
        {
            if (string.IsNullOrEmpty(channelInfoDto.Name))
            {
                return "#".ToString(CultureInfo.InvariantCulture);
            }

            var name = channelInfoDto.Name;
            return SortByNameHeader(name);
        }

        private static string SortByNameHeader(string name)
        {
            var words = name.Split(' ');
            try
            {
                var l = name.ToLower()[0];
                if (words[0].ToLower().Equals("the") ||
                    words[0].ToLower().Equals("a") ||
                    words[0].ToLower().Equals("an"))
                {
                    if (words.Length > 0)
                        l = words[1].ToLower()[0];
                }
                if (l >= 'a' && l <= 'z')
                {
                    return l.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch
            {
            }
            return '#'.ToString(CultureInfo.InvariantCulture);
        }

        internal static string GetPersonType(string type)
        {
            switch (type)
            {
                case "Director":
                    return AppResources.LabelDirector;
                case "Actor":
                    return AppResources.LabelActor;
                case "Writer":
                    return AppResources.LabelWriter;
                case "Producer":
                    return AppResources.LabelProducer;
            }

            return AppResources.LabelOther;
        }

        internal static void CopyItem<T>(T source, T destination) where T : class
        {
            foreach (var sourcePropertyInfo in source.GetType().GetProperties())
            {
                var destPropertyInfo = source.GetType().GetProperty(sourcePropertyInfo.Name);

                destPropertyInfo.SetValue(
                    destination,
                    sourcePropertyInfo.GetValue(source, null),
                    null);
            }
        }

        internal static async Task<List<BaseItemDto>> SortRecentItems(BaseItemDto[] items, bool includeTrailers)
        {
            return await TaskEx.Run(() =>
            {
                var episodesBySeries = items
                    .Where(x => x.Type == "Episode")
                    .GroupBy(l => l.SeriesId)
                    .Select(g => new
                    {
                        Id = g.Key,
                        Name = g.Select(l => l.SeriesName).FirstOrDefault(),
                        Count = g.Count(),
                        CreatedDate = g.OrderByDescending(l => l.DateCreated).First().DateCreated,
                        UserData = new UserItemDataDto { Played = g.All(l => l.UserData.Played) }
                    }).ToList();
                var seriesList = new List<BaseItemDto>();
                if (episodesBySeries.Any())
                {
                    seriesList.AddRange(episodesBySeries.Select(series => new BaseItemDto
                    {
                        Name = String.Format("{0} ({1} items)", series.Name, series.Count),
                        SortName = series.Name,
                        Id = series.Id,
                        DateCreated = series.CreatedDate,
                        Type = "Series",
                        ImageTags = new Dictionary<ImageType, Guid> { { ImageType.Primary, Guid.NewGuid() } },
                        UserData = series.UserData
                    }));
                }

                var tracksByAlbum = items
                    .Where(x => x.Type == "Audio")
                    .GroupBy(x => x.Album)
                    .Select(g => new
                    {
                        Id = g.Select(l => l.ParentId).FirstOrDefault(),
                        Name = g.Key,
                        CreatedDate = g.OrderByDescending(l => l.DateCreated).First().DateCreated,
                        ImageTags = g.SelectMany(x => x.ParentBackdropImageTags ?? new List<Guid>()).Distinct().ToList(),
                        PrimaryImage = g.Select(l => new KeyValuePair<ImageType, Guid>(ImageType.Primary, l.AlbumPrimaryImageTag.HasValue ? l.AlbumPrimaryImageTag.Value : Guid.Empty)).Distinct().ToDictionary(y => y.Key, y => y.Value)
                    }).ToList();
                var albumList = new List<BaseItemDto>();

                if (tracksByAlbum.Any())
                {
                    albumList.AddRange(tracksByAlbum.Select(album => new BaseItemDto
                    {
                        Name = album.Name,
                        Id = album.Id,
                        DateCreated = album.CreatedDate,
                        Type = "MusicAlbum",
                        BackdropImageTags = album.ImageTags,
                        ImageTags = album.PrimaryImage
                    }));
                }

                var recent = items
                    .Where(x => x.Type != "Episode" && x.Type != "Audio")
                    .Union(seriesList)
                    .Union(albumList)
                    .Select(x => x);
                if (!includeTrailers)
                {
                    recent = recent.Where(x => x.Type != "Trailer");
                }
                return recent
                    .OrderByDescending(x => x.DateCreated)
                    .ToList();
            });
        }

        internal static async Task<bool> GetServerConfiguration(IExtendedApiClient apiClient, ILog logger)
        {
            try
            {
                apiClient.ChangeServerLocation(App.Settings.ConnectionDetails.HostName, App.Settings.ConnectionDetails.PortNo);

                logger.Info("Getting server configuration. Hostname ({0}), Port ({1})", apiClient.ServerHostName, apiClient.ServerApiPort);

                var config = await apiClient.GetServerConfigurationAsync();
                App.Settings.ServerConfiguration = config;

                logger.Info("Getting System information");

                var sysInfo = await apiClient.GetSystemInfoAsync();
                App.Settings.SystemStatus = sysInfo;

                logger.Info("Checking if live TV is supported");

#if WP8
                var liveTv = await apiClient.GetLiveTvInfoAsync(default(CancellationToken));
                App.Settings.LiveTvInfo = liveTv;
#endif

                if (SimpleIoc.Default.IsRegistered<ApiWebSocket>())
                {
                    SimpleIoc.Default.Unregister<ApiWebSocket>();
                }

                App.WebSocketClient = await ApiWebSocket.Create(new MBLogger(), new NewtonsoftJsonSerializer(), (ApiClient)apiClient, () => new WebSocketClient(), new CancellationToken());

                return true;
            }
            catch (HttpException ex)
            {
                logger.ErrorException("GetServerConfiguration()", ex);
                return false;
            }
        }

        internal static void CheckProfiles(INavigationService navigationService)
        {
            var clients = App.Settings.ServerConfiguration.ManualLoginClients;
            var loginPage = clients.Contains(ManualLoginCategory.Mobile) ? Constants.Pages.ManualUsernameView : Constants.Pages.ChooseProfileView;

#if WP8
            if (AuthenticationService.Current.IsLoggedIn)
            {
                LockScreenService.Current.Start();
                TileService.Current.UpdatePrimaryTile(App.SpecificSettings.DisplayBackdropOnTile, App.SpecificSettings.UseRichWideTile).ConfigureAwait(false);
            }
#endif
            // If one exists, then authenticate that user.
            navigationService.NavigateTo(AuthenticationService.Current.IsLoggedIn ? TileService.Current.PinnedPage() : loginPage);
        }

        internal static void HandleHttpException(HttpException ex, string message, INavigationService navigationService, ILog log)
        {
            if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show(AppResources.ErrorDisabledUser, AppResources.ErrorDisabledUserTitle, MessageBoxButton.OK);
                log.Error("UnauthorizedAccess for user [{0}]", AuthenticationService.Current.LoggedInUser.Name);
            }
            else
            {
                log.ErrorException(message, ex);
            }
        }

        internal static void HandleHttpException(string message, HttpException ex, INavigationService navigationService, ILog log)
        {
            HandleHttpException(ex, message, navigationService, log);
        }

        internal static async Task<List<PlaylistItem>> ToPlayListItems(this List<BaseItemDto> list, IExtendedApiClient apiClient)
        {
            var newList = new List<PlaylistItem>();
            await Task.Factory.StartNew(() => list.ForEach(item =>
            {
                var playlistItem = item.ToPlaylistItem(apiClient);
                newList.Add(playlistItem);
            }));

            return newList;
        }

        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static string DaysAgo(object value)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            DateTime theDate = (DateTime)value;
            int offset = TimeZoneInfo.Local.BaseUtcOffset.Hours;
            theDate = theDate.AddHours(offset);

            var ts = DateTime.Now.Subtract(theDate);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return ts.Seconds == 1 ? AppResources.LabelOneSecondAgo : String.Format(AppResources.LabelSecondsAgo, ts.Seconds);

            if (seconds < 60 * MINUTE)
                return ts.Minutes == 1 ? AppResources.LabelOneMinuteAgo : String.Format(AppResources.LabelMinutesAgo, ts.Minutes);

            if (seconds < 120 * MINUTE)
                return AppResources.LabelAnHourAgo;

            if (seconds < 24 * HOUR)
                return String.Format(AppResources.LabelHoursAgo, ts.Hours);

            if (seconds < 48 * HOUR)
                return AppResources.LabelYesterday;

            if (seconds < 30 * DAY)
                return String.Format(AppResources.LabelDaysAgo, ts.Days);

            if (seconds < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? AppResources.LabelOneMonthAgo : String.Format(AppResources.LabelMonthsAgo, months);
            }

            return AppResources.LabelDate;
        }

        public static PlaystateCommand ToPlaystateCommandEnum(this string stringCommand)
        {
            PlaystateCommand playState;
#if !WP8
            try
            {
                playState = (PlaystateCommand)Enum.Parse(typeof (PlaystateCommand), stringCommand, true);
            }
            catch
            {
                playState = default(PlaystateCommand);
            }
#else
            Enum.TryParse(stringCommand, out playState);
#endif

            return playState;
        }

        public static bool CanStream(object value)
        {
            if (!App.Settings.LoggedInUser.Configuration.EnableMediaPlayback)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            var item = value as BaseItemDto;
            if (item == null)
            {
                return false;
            }

            if (item.LocationType == LocationType.Virtual || !item.IsVideo)
            {
                return false;
            }

            return true;
        }

        public static async Task<TReturnType> Clone<TReturnType>(this TReturnType item)
        {
#if WP8
            var json = await JsonConvert.SerializeObjectAsync(item);
            return await JsonConvert.DeserializeObjectAsync<TReturnType>(json);
#else
            await TaskEx.Run(() =>
            {
                var json = JsonConvert.SerializeObject(item);
                return JsonConvert.DeserializeObject<TReturnType>(json);
            });
            return item;
#endif
        }
    }
}

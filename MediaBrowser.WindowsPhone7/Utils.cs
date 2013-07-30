using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ailon.WP.Utils;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using Microsoft.Phone.Info;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.Logging;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

#if !WP8
using ScottIsAFool.WindowsPhone;
#endif

namespace MediaBrowser.WindowsPhone
{
    public static class Utils
    {
        public static List<Group<BaseItemPerson>> GroupCastAndCrew(IEnumerable<BaseItemPerson> people)
        {
            var emptyGroups = new List<Group<BaseItemPerson>>();
            var headers = new List<string> { "Director", "Actor", "Writer", "Producer" };
            headers.ForEach(item => emptyGroups.Add(new Group<BaseItemPerson>(item, new List<BaseItemPerson>())));

            var groupedPeople = (from p in people
                                 group p by p.Type
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

        public static List<Group<BaseItemDto>> GroupArtistTracks(IEnumerable<BaseItemDto> tracks)
        {
            var emptyGroups = new List<Group<BaseItemDto>>();

            var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            headers.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item, new List<BaseItemDto>())));

            var groupedTracks = (from t in tracks
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
        }

        internal static string GetSortByNameHeader(BaseItemDto dtoBaseItem)
        {
            var name = !String.IsNullOrEmpty(dtoBaseItem.SortName) ? dtoBaseItem.SortName : dtoBaseItem.Name;
            var words = name.Split(' ');
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
                return l.ToString();
            }
            return '#'.ToString();
        }
        
        internal static void CopyItem<T>(T source, T destination) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public);

            foreach (var fi in properties)
            {
                if (fi.CanWrite)
                {
                    fi.SetValue(destination, fi.GetValue(source, null), null);
                }
            }
        }

        internal static async Task<List<BaseItemDto>> SortRecentItems(BaseItemDto[] items)
        {
            var episodesBySeries = items
                    .Where(x => x.Type == "Episode")
                    .GroupBy(l => l.SeriesId)
                    .Select(g => new
                    {
                        Id = g.Key,
                        Name = g.Select(l => l.SeriesName).FirstOrDefault(),
                        Count = g.Count(),
                        CreatedDate = g.OrderByDescending(l => l.DateCreated).First().DateCreated
                    }).ToList();
            var seriesList = new List<BaseItemDto>();
            if (episodesBySeries.Any())
            {
                seriesList.AddRange(episodesBySeries.Select(series => new BaseItemDto
                {
                    Name = String.Format("{0} ({1} items)", series.Name, series.Count),
                    Id = series.Id,
                    DateCreated = series.CreatedDate,
                    Type = "Series",
                    SortName = Constants.Messages.GetTvInformationMsg,
                    ImageTags = new Dictionary<ImageType, Guid> { { ImageType.Primary, Guid.NewGuid() } }
                }));
            }

            var tracksByAlbum = items
                .Where(x => x.Type == "Audio")
                .GroupBy(x => x.Album)
                .Select(g => new
                                 {
                                     Id = g.Select(l => l.ParentId).FirstOrDefault(),
                                     Name = g.Key,
                                     CreatedDate = g.OrderByDescending(l => l.DateCreated).First().DateCreated
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
                                                                     }));
            }

            var recent = items
                .Where(x => x.Type != "Episode" && x.Type != "Audio")
                .Union(seriesList)
                .Union(albumList)
                .Select(x => x);
            if (!App.SpecificSettings.IncludeTrailersInRecent)
            {
                recent = recent.Where(x => x.Type != "Trailer");
            }
            return recent
                .OrderByDescending(x => x.DateCreated)
                .Take(6)
                .ToList();

        }

        internal static async Task<bool> GetServerConfiguration(ExtendedApiClient apiClient, ILog logger)
        {
            try
            {
                apiClient.ServerHostName = App.Settings.ConnectionDetails.HostName;
                apiClient.ServerApiPort = App.Settings.ConnectionDetails.PortNo;
                
                logger.Info("Getting server configuration. Hostname ({0}), Port ({1})", apiClient.ServerHostName, apiClient.ServerApiPort);
                
                var config = await apiClient.GetServerConfigurationAsync();
                App.Settings.ServerConfiguration = config;
                
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
            // If one exists, then authenticate that user.
            navigationService.NavigateTo(AuthenticationService.Current.IsLoggedIn ? Constants.Pages.HomePage : loginPage);
        }

        internal static string GetDeviceName()
        {
            var deviceName = DeviceStatus.DeviceName;
            var deviceId = DeviceStatus.DeviceManufacturer;
            var phone = PhoneNameResolver.Resolve(deviceId, deviceName);
            var deviceInfo = string.Format("{0} ({1})", phone.CanonicalModel, phone.CanonicalManufacturer);

            return deviceInfo;
        }

        internal static string GetDeviceId()
        {
            var uniqueId = SimpleIoc.Default.GetInstance<IDeviceExtendedPropertiesService>().DeviceUniqueId;
            var deviceId = Convert.ToBase64String(uniqueId, 0, uniqueId.Length);

            return deviceId;
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
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (seconds < 60 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (seconds < 120 * MINUTE)
                return "an hour ago";

            if (seconds < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (seconds < 48 * HOUR)
                return "yesterday";

            if (seconds < 30 * DAY)
                return ts.Days + " days ago";

            if (seconds < 12 * MONTH)
            {
                int months = System.Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            return "date";
        }
    }
}

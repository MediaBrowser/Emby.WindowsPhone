using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ailon.WP.Utils;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using Microsoft.Phone.Info;

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

        internal static async Task Login(UserDto selectedUser, string pinCode, Action successAction)
        {
            var client = SimpleIoc.Default.GetInstance<ExtendedApiClient>();

            try
            {
                await client.AuthenticateUserAsync(selectedUser.Id, pinCode.ToHash());

                if (successAction != null)
                {
                    successAction.Invoke();
                }
            }
            catch (HttpException ex)
            {
                var v = "";
            }
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
                    SortName = Constants.GetTvInformationMsg,
                    ImageTags = new Dictionary<ImageType, Guid> { { ImageType.Primary, Guid.NewGuid() } }
                }));
            }
            var recent = items
                .Where(x => x.Type != "Episode")
                .Union(seriesList)
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

        internal static async Task<bool> GetServerConfiguration(ExtendedApiClient ApiClient)
        {
            try
            {
                ApiClient.ServerHostName = App.Settings.ConnectionDetails.HostName;
                ApiClient.ServerApiPort = App.Settings.ConnectionDetails.PortNo;
                var config = await ApiClient.GetServerConfigurationAsync();
                App.Settings.ServerConfiguration = config;
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static async Task CheckProfiles(INavigationService NavigationService)
        {
            // If one exists, then authenticate that user.
            if (App.Settings.LoggedInUser != null)
            {
                await Login(App.Settings.LoggedInUser, App.Settings.PinCode, () =>
                {
                    if (!String.IsNullOrEmpty(App.Action))
                        NavigationService.NavigateToPage(App.Action);
                    else
                        NavigationService.NavigateToPage("/Views/MainPage.xaml");
                });
            }
            else
            {
                NavigationService.NavigateToPage("/Views/ChooseProfileView.xaml");
            }
        }

        internal static byte[] ToHash(this string pinCode)
        {
            var sha1 = new SHA1Managed();
            var encoding = new UTF8Encoding();
            sha1.ComputeHash(encoding.GetBytes(pinCode));

            return sha1.Hash;
        }

        internal static ExtendedApiClient SetDeviceProperties(this ExtendedApiClient apiClient)
        {
            var deviceName = DeviceStatus.DeviceName;
            var deviceId = DeviceStatus.DeviceManufacturer;

            var phone = PhoneNameResolver.Resolve(deviceId, deviceName);

            var deviceInfo = String.Format("{0} ({1})", phone.CanonicalModel, phone.CanonicalManufacturer);

            apiClient.DeviceName = deviceInfo;

            object uniqueId;
            DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId);
            apiClient.DeviceId = Convert.ToBase64String((byte[])uniqueId, 0, ((byte[])uniqueId).Length);

            return apiClient;
        }
    }
}

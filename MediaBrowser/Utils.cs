using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.DTO;
using MediaBrowser.Model.Entities;
using MediaBrowser.WindowsPhone.Model;
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

        internal static async Task Login(DtoUser selectedUser, string pinCode, Action successAction)
        {
            var client = SimpleIoc.Default.GetInstance<ExtendedApiClient>();
            await client.AuthenticateUserAsync(selectedUser.Id, pinCode);

            
                if(successAction != null)
                {
                    successAction.Invoke();
                }
            
        }
        
        internal static void CopyItem<T>(T source, T destination) where T : class
        {
            var type = typeof (T);
            var properties = type.GetProperties(BindingFlags.Public);

            foreach (var fi in properties)
            {
                if (fi.CanWrite)
                {
                    fi.SetValue(destination, fi.GetValue(source, null), null);
                }
            }
        }

        internal static async Task<List<DtoBaseItem>> SortRecentItems(DtoBaseItem[] items)
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
            var seriesList = new List<DtoBaseItem>();
            if (episodesBySeries.Any())
            {
                seriesList.AddRange(episodesBySeries.Select(series => new DtoBaseItem
                {
                    Name = string.Format("{0} ({1} items)", series.Name, series.Count),
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
            recent
                .OrderByDescending(x => x.DateCreated)
                .Take(6);
            return recent.ToList();
        }
    }
}

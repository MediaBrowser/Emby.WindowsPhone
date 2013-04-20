using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MetroLog;
using Win8nl.Utilities;

namespace MediaBrowser.Windows8
{
    public class Utils
    {
        public static async Task DoLogin(ILogger logger, UserDto selectedUser, string pinCode, Action successAction)
        {
            var client = SimpleIoc.Default.GetInstance<ExtendedApiClient>();
            logger.Info("Authenticating user [{0}]", selectedUser.Name);

            try
            {
                await client.AuthenticateUserAsync(selectedUser.Id, pinCode.ToHash());

                logger.Info("Logged in as [{0}]", selectedUser.Name);

                if (successAction != null)
                {
                    successAction.Invoke();
                }
            }
            catch (HttpException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    if (ex.StatusCode.Value == HttpStatusCode.Unauthorized)
                    {
                        logger.Info("Login unsuccessful: Incorrect username or password");
                        Messenger.Default.Send(new NotificationMessage(selectedUser.Id, Constants.ErrorLoggingInMsg));        
                    }
                }
            }
        }

        public static async Task CopyItem<T>(T source, T destination) where T : class
        {
            await Task.Run(() =>
            {
                var type = typeof(T);
                var myObjectFields = type.GetRuntimeProperties();

                foreach (var fi in myObjectFields)
                {
                    if (fi.CanWrite)
                        fi.SetValue(destination, fi.GetValue(source));
                }
            });
        }

        public async static Task<ObservableCollection<Group<BaseItemPerson>>> GroupCastAndCrew(BaseItemDto item)
        {
            var castAndCrew = new ObservableCollection<Group<BaseItemPerson>>
                                  {
                                  new Group<BaseItemPerson> {Title = "Director"},
                                  new Group<BaseItemPerson> {Title = "Cast"}
                              };
            await Task.Run(() =>
                               {
                                   if (item.People != null && item.People.Any())
                                   {

                                       var directors = item.People
                                                           .Where(x => x.Type.Equals("Director"))
                                                           .Select(x => x).ToList();
                                       foreach (var director in directors)
                                       {
                                           castAndCrew[0].Items.Add(director);
                                       }

                                       var castMembers = item.People
                                                             .Where(x => x.Type.Equals("Actor"))
                                                             .Select(x => x);
                                       foreach (var cast in castMembers)
                                       {
                                           castAndCrew[1].Items.Add(cast);
                                       }
                                   }
                               });

            return castAndCrew;
        }

        public static List<Group<BaseItemDto>> GroupArtistTracks(IEnumerable<BaseItemDto> tracks)
        {
            var emptyGroups = new List<Group<BaseItemDto>>();

            var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            headers.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>{Title = item, Items = new ObservableCollection<BaseItemDto>()}));

            var groupedTracks = (from t in tracks
                                 group t by GetSortByNameHeader(t)
                                     into grp
                                     orderby grp.Key
                                     select new Group<BaseItemDto> { Title = grp.Key, Items = new ObservableCollection<BaseItemDto>(grp.ToList()) }).ToList();

            var result = (from g in groupedTracks.Union(emptyGroups)
                          where g.Items.Count > 0
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
    }
}

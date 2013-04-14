using System;
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

namespace MediaBrowser.Windows8
{
    public class Utils
    {
        public static async Task DoLogin(UserDto selectedUser, string pinCode, Action successAction)
        {
            var client = SimpleIoc.Default.GetInstance<ExtendedApiClient>();
            LogManagerFactory.DefaultLogManager.GetLogger<Utils>().Info("Logging in as " + selectedUser.Name);

            try
            {
                await client.AuthenticateUserAsync(selectedUser.Id, pinCode.ToHash());
                if (successAction != null)
                {
                    LogManagerFactory.DefaultLogManager.GetLogger<Utils>().Info("Login successful");
                    successAction.Invoke();
                }
            }
            catch (HttpException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    if (ex.StatusCode.Value == HttpStatusCode.Unauthorized)
                    {
                        LogManagerFactory.DefaultLogManager.GetLogger<Utils>().Info("Login unsuccessful: Incorrect username or password");
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

        
    }
}

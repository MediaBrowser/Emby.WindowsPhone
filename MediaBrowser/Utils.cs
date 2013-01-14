using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.DTO;
using ScottIsAFool.WindowsPhone;

namespace MediaBrowser.WindowsPhone
{
    public static class Utils
    {
        public static List<Group<BaseItemPerson>> GroupCastAndCrew(IEnumerable<BaseItemPerson> people)
        {
            var result = new List<Group<BaseItemPerson>>();
            var emptyGroups = new List<Group<BaseItemPerson>>();
            var headers = new List<string> { "Actor", "Director", "Writer", "Producer" };
            headers.ForEach(item => emptyGroups.Add(new Group<BaseItemPerson>(item, new List<BaseItemPerson>())));

            var groupedPeople = (from p in people
                                 group p by p.Type
                                     into grp
                                     orderby grp.Key
                                     select new Group<BaseItemPerson>(grp.Key, grp)).ToList();

            result = (from g in groupedPeople.Union(emptyGroups)
                           orderby g.Title
                           select g).ToList();

            return result;
        }

        internal static async Task Login(DtoUser selectedUser, string pinCode, Action successAction)
        {
            var client = SimpleIoc.Default.GetInstance<ApiClient>();
            var result = await client.AuthenticateUserAsync(selectedUser.Id, pinCode);

            if (result.Success)
            {
                if(successAction != null)
                {
                    successAction.Invoke();
                }
            }
            else
            {
                App.ShowMessage("", "Error logging in");
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
    }
}

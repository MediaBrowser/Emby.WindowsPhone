using System.Collections.Generic;
using System.Linq;
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
    }
}

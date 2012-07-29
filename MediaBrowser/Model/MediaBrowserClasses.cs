
using System.Collections.Generic;
namespace MediaBrowser.Model
{
    public class MyBaseItem : MediaBrowser.Model.Entities.BaseItem
    {
    }

    public class RootObject<T> : RootObject where T : class
    {
        public List<T> Children { get; set; }
        
    }

    public class RootObject
    {
        public MyBaseItem Item { get; set; }
        public string Type { get; set; }
    }
}

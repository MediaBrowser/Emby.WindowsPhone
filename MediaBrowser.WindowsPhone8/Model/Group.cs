using System.Collections.Generic;

namespace MediaBrowser.WindowsPhone.Model
{
    public class Group<T> : List<T>
    {
        public Group(string name, IEnumerable<T> items)
            : base(items)
        {
            this.Title = name;
        }

        public string Title
        {
            get;
            set;
        }
    }
}

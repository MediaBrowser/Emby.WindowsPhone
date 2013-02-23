using System.Collections.Generic;

namespace MediaBrowser.Windows8.ViewModel
{
    public class GroupInfoList<T> : List<object>
    {
        public object Key { get; set; }

        public new IEnumerator<object> GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}
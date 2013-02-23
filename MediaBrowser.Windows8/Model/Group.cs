using System.Collections.ObjectModel;

namespace MediaBrowser.Windows8.Model
{
    public class Group<T> : ModelBase where T : class
    {
        public Group ()
        {
            Items = new ObservableCollection<T>();
        }
        public string Title { get; set; }
        public ObservableCollection<T> Items { get; set; }
    }
}

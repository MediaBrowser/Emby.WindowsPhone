namespace MediaBrowser.Windows8.Model
{
    public class GridItemWrapper<T>
    {
        public GridItemWrapper()
        {
            RowSpan = 1;
            ColSpan = 1;
        } 

        public GridItemWrapper(T item) : base()
        {
            Item = item;
        }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public T Item { get; set; }
    }
}

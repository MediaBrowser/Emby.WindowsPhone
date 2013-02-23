using MediaBrowser.Model.Dto;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaBrowser.Windows8
{
    public class TvItemsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PosterDataTemplate { get; set; }
        public DataTemplate SeasonDataTemplate { get; set; }
        public DataTemplate EpisodeDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var dtoItem = (BaseItemDto) item;
            if(dtoItem.Type.Equals("Season"))
            {
                return SeasonDataTemplate;
            }
            else if(dtoItem.Type.Equals("Episode"))
            {
                return EpisodeDataTemplate;
            }
            else if(dtoItem.Type.Equals("Series"))
            {
                return PosterDataTemplate;
            }
            else
            {
                return base.SelectTemplateCore(item, container);
            }
        }
    }
}

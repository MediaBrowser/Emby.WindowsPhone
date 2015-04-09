using System.Windows;
using MediaBrowser.Model.Dto;
using Telerik.Windows.Controls;

namespace Emby.WindowsPhone.Model
{
    public class PlaylistTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenericVideo { get; set; }
        public DataTemplate Movie { get; set; }
        public DataTemplate Episode { get; set; }
        public DataTemplate Audio { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var mbItem = item as BaseItemDto;
            if (mbItem == null)
            {
                return base.SelectTemplate(item, container);
            }

            switch (mbItem.Type)
            {
                case "Movie":
                    return Movie;
                case "Episode":
                    return Episode;
                case "Audio":
                    return Audio;
                default:
                    return GenericVideo;
            }
        }
    }
}

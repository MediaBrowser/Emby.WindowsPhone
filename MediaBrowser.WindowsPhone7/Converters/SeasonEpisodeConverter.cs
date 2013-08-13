using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class SeasonEpisodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var episode = (BaseItemDto) value;
            var episodeNumber = episode.IndexNumber;
            var seriesNumber = episode.ParentIndexNumber;

            return string.Format("{0:00}x{1:00}", seriesNumber, episodeNumber);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

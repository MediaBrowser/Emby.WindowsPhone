using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class EpisodeNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var item = value as BaseItemDto;
            if (item == null)
            {
                return string.Empty;
            }

            if (item.IndexNumberEnd.HasValue)
            {
                return string.Format("{0}-{1}", item.IndexNumber, item.IndexNumberEnd);
            }

            return item.IndexNumber.HasValue ? item.IndexNumber.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EpisodeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var item = value as BaseItemDto;
            if (item == null)
            {
                return string.Empty;
            }

            var episodeNumber = new EpisodeNumberConverter().Convert(item, targetType, null, culture).ToString();
            var seasonNumber = item.ParentIndexNumber;
            var episodeName = item.Name;

            return string.Format("{0}x{1} - {2}", seasonNumber, episodeNumber, episodeName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

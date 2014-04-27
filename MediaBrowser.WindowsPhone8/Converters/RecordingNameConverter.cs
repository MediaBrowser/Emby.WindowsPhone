using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class RecordingNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var item = value as RecordingInfoDto;
            if (item == null)
            {
                return string.Empty;
            }

            return string.IsNullOrEmpty(item.EpisodeTitle) ? item.Name : item.EpisodeTitle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

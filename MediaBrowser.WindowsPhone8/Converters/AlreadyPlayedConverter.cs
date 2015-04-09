using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class AlreadyPlayedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            var item = value as BaseItemDto;
            if (item == null || item.UserData == null || item.UserData.PlaybackPositionTicks == 0 || !item.RunTimeTicks.HasValue)
            {
                return 0;
            }

            var percentage = (double)((double)item.UserData.PlaybackPositionTicks / (double)item.RunTimeTicks.Value) * 100;

            return percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

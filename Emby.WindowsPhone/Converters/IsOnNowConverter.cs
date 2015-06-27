using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class IsOnNowConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as BaseItemDto;
            if (item == null)
            {
                return false;
            }

            var now = DateTime.Now;
            return item.EndDate.HasValue && item.StartDate.HasValue && now < item.EndDate.Value.ToLocalTime() && now > item.StartDate.Value.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

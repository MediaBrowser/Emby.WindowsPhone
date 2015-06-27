using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class HasAiredConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var programme = value as BaseItemDto;
            if (programme == null || !programme.EndDate.HasValue)
            {
                return true;
            }

            var endTime = programme.EndDate.Value.ToLocalTime();
            var now = DateTime.Now;

            return now > endTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

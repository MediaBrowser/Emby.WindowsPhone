using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;

namespace Emby.WindowsPhone.Converters
{
    public class IsOnNowConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var item = value as ProgramInfoDto;
            if (item == null)
            {
                return false;
            }

            var now = DateTime.Now;
            return now < item.EndDate.ToLocalTime() && now > item.StartDate.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

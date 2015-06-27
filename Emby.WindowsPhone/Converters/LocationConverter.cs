using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Emby.WindowsPhone.Converters
{
    public class LocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var locations = (List<string>) value;

            return string.Join(", ", locations);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
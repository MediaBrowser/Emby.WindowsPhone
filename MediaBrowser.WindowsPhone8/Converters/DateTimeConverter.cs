using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime?) value;
            if (!date.HasValue)
            {
                return string.Empty;
            }

            return date.Value.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
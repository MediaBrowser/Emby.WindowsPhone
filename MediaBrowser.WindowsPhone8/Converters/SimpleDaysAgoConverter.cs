using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class SimpleDaysAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Utils.DaysAgo(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace Emby.WindowsPhone.Converters
{
    public class SecondsToMinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var seconds = (int) value;

            return seconds/60;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var minutes = (double) value;

            return (int) (minutes*60);
        }
    }
}

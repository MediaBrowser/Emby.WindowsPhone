using System;
using System.Globalization;
using System.Windows.Data;

namespace Emby.WindowsPhone.Converters
{
    public class NullBooleanConverter : IValueConverter
    {
        public bool Inverted { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Inverted)
            {
                return value == null;
            }

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
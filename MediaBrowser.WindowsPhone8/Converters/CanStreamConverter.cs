using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class CanStreamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if !WP8
            return false;
#else
            return Utils.CanStream(value);
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CanStreamVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if !WP8
            return Visibility.Collapsed;
#else
            return Utils.CanStream(value) ? Visibility.Visible : Visibility.Collapsed;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

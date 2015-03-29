using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Shell;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class AppBarModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? ApplicationBarMode.Minimized : ApplicationBarMode.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
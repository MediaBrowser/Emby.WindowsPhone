using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class RequiresServerVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Collapsed;
            }

            return Utils.IsNewerThanServerVersion(value.ToString(), parameter.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
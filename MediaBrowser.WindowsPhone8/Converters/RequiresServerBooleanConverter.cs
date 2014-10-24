using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class RequiresServerBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            return Utils.IsNewerThanServerVersion(value.ToString(), parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class LoggedInDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                return "Profile last used " + Utils.DaysAgo(value);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

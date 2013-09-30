using System;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class PlaylistCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var count = int.Parse(value.ToString());

            if (parameter != null && bool.Parse(parameter.ToString()))
            {
                return count > 0;
            }

            return count > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

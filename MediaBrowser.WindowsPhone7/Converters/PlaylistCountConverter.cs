using System;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class PlaylistCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var count = int.Parse(value.ToString());

            return count > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

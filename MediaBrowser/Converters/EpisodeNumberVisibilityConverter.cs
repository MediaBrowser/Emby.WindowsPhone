using System;
using System.Windows;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class EpisodeNumberVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string) value).ToLower().Equals("episode") ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Emby.WindowsPhone.Converters
{
    public class CollectionVisibilityConverter : IValueConverter
    {
        public bool Inverted { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Inverted ? Visibility.Collapsed : Visibility.Visible;
            }

            var collection = value as IList;
            if (collection == null)
            {
                return Inverted ? Visibility.Collapsed : Visibility.Visible;
            }

            return collection.Count > 0 && !Inverted ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
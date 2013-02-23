using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class CollectionDateHider : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                var type = (string) value;
                return type.Equals("CollectionFolder") || type.Equals("TrailerCollectionFolder") ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

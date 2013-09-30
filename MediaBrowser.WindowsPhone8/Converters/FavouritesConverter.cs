using System;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class FavouritesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var textOrIcon = ((string) parameter).Equals("text");
            if (value == null)
            {
                if(textOrIcon)return "add";
                return new Uri("/Icons/appbar.star.add.png", UriKind.Relative);
            }
            var isFavourite = (bool) value;
            if (textOrIcon)
            {
                return isFavourite ? "remove" : "add";
            }
            return isFavourite ? new Uri("/Icons/appbar.star.minus.png", UriKind.Relative) : new Uri("/Icons/appbar.star.add.png", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class ShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateType = "shortdate";
            if (parameter != null) dateType = (string) parameter;
            if(value != null)
            {
                CultureInfo info = CultureInfo.CurrentUICulture;
                if (dateType.Equals("shortdate"))
                    return ((DateTime) value).ToString(info.DateTimeFormat.ShortDatePattern);
                return ((DateTime) value).ToString(info.DateTimeFormat.LongDatePattern);
            }
            return "no date";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

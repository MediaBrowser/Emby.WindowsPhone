using System;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class StringFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(parameter != null)
            {
                string formatterString = parameter.ToString();

                if(!string.IsNullOrEmpty(formatterString))
                {
                    return string.Format(formatterString, value);
                }
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
